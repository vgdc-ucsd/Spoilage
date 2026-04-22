using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TextboxControl.Animation;

namespace TextboxControl
{
    public class TextAnimator
    {
        private readonly TMP_Text _target;
        private readonly Reducer _reducer;
        private readonly Reducer _previewReducer;

        private Vector3[][] _baselineVerts;
        private Color32[][] _baselineColors;
        private bool _baselineValid;

        private int _totalChars;
        private bool _pushedStaticFrame;
        private bool _anyAnim;
        private int _lastRegionVersion = -1;

        private readonly Vector3[] _quadVerts = new Vector3[4];
        private readonly Color32[] _quadColors = new Color32[4];

        private int[] _activeRegionIndices = new int[16];
        private int _activeRegionCount;

        public TextAnimator(TMP_Text target, Reducer reducer)
        {
            _target = target;
            _reducer = reducer;

            _previewReducer = new Reducer();
            _previewReducer.OnError += _ => { };
            _previewReducer.LogExternalControls = false;
        }

        public void MarkLayoutDirty()
        {
            _baselineValid = false;
            _pushedStaticFrame = false;
        }

        public void Prepare(string source)
        {
            if (_target == null)
            {
                return;
            }

            _previewReducer.Play(source, previewMode: true);
            _previewReducer.Skip();

            _totalChars = _previewReducer.RevealedCount;
            _target.text = TMPFormatter.Build(_previewReducer.DisplayBuffer, _previewReducer.StyleRuns);
            _target.ForceMeshUpdate();

            MarkLayoutDirty();
            SnapshotBaseline();
            _lastRegionVersion = -1;
            Render();
        }

        public void Clear()
        {
            MarkLayoutDirty();
            _totalChars = 0;
            _lastRegionVersion = -1;

            if (_target != null)
            {
                _target.text = string.Empty;
            }
        }

        public void Render()
        {
            if (_target == null)
            {
                return;
            }

            EnsureBaseline();

            int revealed = _reducer.RevealedCount;
            List<Region> regions = _reducer.RegionsDirect;
            RefreshAnimationState(regions);

            bool isStatic = revealed >= _totalChars && !_anyAnim;
            if (isStatic && _pushedStaticFrame)
            {
                return;
            }

            TMP_TextInfo textInfo = _target.textInfo;
            RenderCharacters(textInfo, regions, revealed, (float)_reducer.TimeSincePlay);

            _target.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
            _pushedStaticFrame = isStatic;
        }

        private void RefreshAnimationState(List<Region> regions)
        {
            if (_reducer.RegionVersion == _lastRegionVersion)
            {
                return;
            }

            _anyAnim = false;
            for (int i = 0; i < regions.Count; i++)
            {
                if (regions[i].Animation != null)
                {
                    _anyAnim = true;
                    break;
                }
            }

            _lastRegionVersion = _reducer.RegionVersion;
        }

        private void RenderCharacters(TMP_TextInfo textInfo, List<Region> regions, int revealed, float now)
        {
            int regCount = regions.Count;
            int charCount = textInfo.characterCount;

            _activeRegionCount = 0;
            int nextRegionIndex = 0;

            for (int charIndex = 0; charIndex < charCount; charIndex++)
            {
                UpdateActiveRegions(charIndex, regions, regCount, ref nextRegionIndex);

                TMP_CharacterInfo ci = textInfo.characterInfo[charIndex];
                if (!ci.isVisible)
                {
                    continue;
                }

                if (!TryGetMeshBuffers(textInfo, ci, out MeshBuffers mesh))
                {
                    continue;
                }

                if (charIndex >= revealed)
                {
                    HideCharacter(mesh);
                    continue;
                }

                CharAnimState state = CharAnimState.Identity;
                bool hasAnim = ApplyStateAnimations(charIndex, now, regions, ref state);

                if (hasAnim)
                {
                    ApplyStateToQuad(
                        state,
                        mesh.SourceVertices,
                        mesh.SourceColors,
                        mesh.DestinationVertices,
                        mesh.DestinationColors,
                        mesh.VertexIndex);
                }
                else
                {
                    RestoreBaselineQuad(mesh);
                }

                ApplyVertexAnimations(charIndex, now, regions, mesh);
            }
        }

        private void UpdateActiveRegions(int charIndex, List<Region> regions, int regionCount, ref int nextRegionIndex)
        {
            while (nextRegionIndex < regionCount && regions[nextRegionIndex].Start <= charIndex)
            {
                Region candidate = regions[nextRegionIndex];
                if (candidate.Animation != null && charIndex < candidate.End)
                {
                    EnsureRegionCapacity(ref _activeRegionIndices, _activeRegionCount + 1);
                    _activeRegionIndices[_activeRegionCount++] = nextRegionIndex;
                }

                nextRegionIndex++;
            }

            PruneInactiveRegions(charIndex, regions, _activeRegionIndices, ref _activeRegionCount);
        }

        private bool ApplyStateAnimations(int charIndex, float now, List<Region> regions, ref CharAnimState state)
        {
            bool hasCharAnimation = false;

            for (int i = 0; i < _activeRegionCount; i++)
            {
                Region region = regions[_activeRegionIndices[i]];
                IAnimation animation = region.Animation;
                if (animation == null)
                {
                    continue;
                }

                int inRegion = charIndex - region.Start;
                if ((uint)inRegion >= (uint)region.Length)
                {
                    continue;
                }

                animation.Apply(ref state, BuildContext(charIndex, inRegion, region, now));
                hasCharAnimation = true;
            }

            return hasCharAnimation;
        }

        private void ApplyVertexAnimations(int charIndex, float now, List<Region> regions, MeshBuffers mesh)
        {
            for (int i = 0; i < _activeRegionCount; i++)
            {
                Region region = regions[_activeRegionIndices[i]];
                if (!(region.Animation is IVertexAnimation va))
                {
                    continue;
                }

                int inRegion = charIndex - region.Start;
                AnimationContext ctx = BuildContext(charIndex, inRegion, region, now);

                CopyQuad(mesh.DestinationVertices, mesh.DestinationColors, mesh.VertexIndex, _quadVerts, _quadColors);
                va.ApplyVertices(_quadVerts, _quadColors, ctx);
                WriteQuad(mesh.DestinationVertices, mesh.DestinationColors, mesh.VertexIndex, _quadVerts, _quadColors);
            }
        }

        private static AnimationContext BuildContext(int charIndex, int inRegion, Region region, float now)
        {
            return new AnimationContext
            {
                CharIndexInBuffer = charIndex,
                CharIndexInRegion = inRegion,
                RegionLength = region.Length,
                TimeSinceRegionStart = now - (float)region.StartTime,
            };
        }

        private static void CopyQuad(
            Vector3[] srcV,
            Color32[] srcC,
            int vertexIndex,
            Vector3[] dstV,
            Color32[] dstC)
        {
            for (int i = 0; i < 4; i++)
            {
                dstV[i] = srcV[vertexIndex + i];
                dstC[i] = srcC[vertexIndex + i];
            }
        }

        private static void WriteQuad(
            Vector3[] dstV,
            Color32[] dstC,
            int vertexIndex,
            Vector3[] srcV,
            Color32[] srcC)
        {
            for (int i = 0; i < 4; i++)
            {
                dstV[vertexIndex + i] = srcV[i];
                dstC[vertexIndex + i] = srcC[i];
            }
        }

        private static void HideCharacter(MeshBuffers mesh)
        {
            for (int i = 0; i < 4; i++)
            {
                int quadIndex = mesh.VertexIndex + i;
                mesh.DestinationVertices[quadIndex] = mesh.SourceVertices[quadIndex];

                Color32 color = mesh.SourceColors[quadIndex];
                mesh.DestinationColors[quadIndex] = new Color32(color.r, color.g, color.b, 0);
            }
        }

        private static void RestoreBaselineQuad(MeshBuffers mesh)
        {
            for (int i = 0; i < 4; i++)
            {
                int quadIndex = mesh.VertexIndex + i;
                mesh.DestinationVertices[quadIndex] = mesh.SourceVertices[quadIndex];
                mesh.DestinationColors[quadIndex] = mesh.SourceColors[quadIndex];
            }
        }

        private bool TryGetMeshBuffers(TMP_TextInfo textInfo, TMP_CharacterInfo ci, out MeshBuffers mesh)
        {
            mesh = default;

            int materialIndex = ci.materialReferenceIndex;
            int vertexIndex = ci.vertexIndex;
            if (materialIndex < 0 || materialIndex >= textInfo.meshInfo.Length)
            {
                return false;
            }

            Vector3[] srcV = _baselineVerts[materialIndex];
            Color32[] srcC = _baselineColors[materialIndex];
            Vector3[] dstV = textInfo.meshInfo[materialIndex].vertices;
            Color32[] dstC = textInfo.meshInfo[materialIndex].colors32;

            if (srcV == null || srcC == null || dstV == null || dstC == null)
            {
                return false;
            }

            if (vertexIndex < 0 ||
                vertexIndex + 3 >= srcV.Length ||
                vertexIndex + 3 >= srcC.Length ||
                vertexIndex + 3 >= dstV.Length ||
                vertexIndex + 3 >= dstC.Length)
            {
                return false;
            }

            mesh = new MeshBuffers
            {
                VertexIndex = vertexIndex,
                SourceVertices = srcV,
                SourceColors = srcC,
                DestinationVertices = dstV,
                DestinationColors = dstC,
            };

            return true;
        }

        private void EnsureBaseline()
        {
            if (_baselineValid && !_target.havePropertiesChanged)
            {
                return;
            }

            _target.ForceMeshUpdate();
            SnapshotBaseline();
            _pushedStaticFrame = false;
        }

        private void SnapshotBaseline()
        {
            TMP_TextInfo textInfo = _target.textInfo;
            int subMeshCount = textInfo.meshInfo == null ? 0 : textInfo.meshInfo.Length;

            if (_baselineVerts == null || _baselineVerts.Length != subMeshCount)
            {
                _baselineVerts = new Vector3[subMeshCount][];
                _baselineColors = new Color32[subMeshCount][];
            }

            for (int materialIndex = 0; materialIndex < subMeshCount; materialIndex++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
                int vertexCount = meshInfo.vertices == null ? 0 : meshInfo.vertices.Length;

                if (_baselineVerts[materialIndex] == null || _baselineVerts[materialIndex].Length != vertexCount)
                {
                    _baselineVerts[materialIndex] = new Vector3[vertexCount];
                }

                if (_baselineColors[materialIndex] == null || _baselineColors[materialIndex].Length != vertexCount)
                {
                    _baselineColors[materialIndex] = new Color32[vertexCount];
                }

                if (vertexCount > 0)
                {
                    Array.Copy(meshInfo.vertices, _baselineVerts[materialIndex], vertexCount);
                    Array.Copy(meshInfo.colors32, _baselineColors[materialIndex], vertexCount);
                }
            }

            _baselineValid = true;
        }

        private static void PruneInactiveRegions(
            int charIndex,
            List<Region> regions,
            int[] activeRegionIndices,
            ref int activeRegionCount)
        {
            int write = 0;
            for (int read = 0; read < activeRegionCount; read++)
            {
                int regionIndex = activeRegionIndices[read];
                if (charIndex < regions[regionIndex].End)
                {
                    activeRegionIndices[write++] = regionIndex;
                }
            }

            activeRegionCount = write;
        }

        private static void EnsureRegionCapacity(ref int[] indices, int required)
        {
            if (indices.Length >= required)
            {
                return;
            }

            int newSize = indices.Length;
            while (newSize < required)
            {
                newSize *= 2;
            }

            Array.Resize(ref indices, newSize);
        }

        private static void ApplyStateToQuad(
            CharAnimState state,
            Vector3[] srcVerts,
            Color32[] srcColors,
            Vector3[] dstVerts,
            Color32[] dstColors,
            int vertexIndex)
        {
            Vector3 center =
                (srcVerts[vertexIndex] + srcVerts[vertexIndex + 1] + srcVerts[vertexIndex + 2] + srcVerts[vertexIndex + 3]) *
                0.25f;

            bool hasRotation = state.Rotation != 0f;
            float cos = 1f;
            float sin = 0f;
            if (hasRotation)
            {
                float radians = state.Rotation * Mathf.Deg2Rad;
                cos = Mathf.Cos(radians);
                sin = Mathf.Sin(radians);
            }

            bool hasScale = state.Scale.x != 1f || state.Scale.y != 1f;
            bool hasOffset = state.PositionOffset.x != 0f || state.PositionOffset.y != 0f;
            bool hasTint = state.ColorTint.r != 1f ||
                           state.ColorTint.g != 1f ||
                           state.ColorTint.b != 1f ||
                           state.ColorTint.a != 1f;

            int tintR = 255;
            int tintG = 255;
            int tintB = 255;
            int tintA = 255;
            if (hasTint)
            {
                tintR = (int)(state.ColorTint.r * 255f + 0.5f);
                tintG = (int)(state.ColorTint.g * 255f + 0.5f);
                tintB = (int)(state.ColorTint.b * 255f + 0.5f);
                tintA = (int)(state.ColorTint.a * 255f + 0.5f);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 vertex = srcVerts[vertexIndex + i];

                if (hasScale || hasRotation)
                {
                    Vector3 delta = vertex - center;

                    if (hasScale)
                    {
                        delta.x *= state.Scale.x;
                        delta.y *= state.Scale.y;
                    }

                    if (hasRotation)
                    {
                        float rotatedX = delta.x * cos - delta.y * sin;
                        float rotatedY = delta.x * sin + delta.y * cos;
                        delta.x = rotatedX;
                        delta.y = rotatedY;
                    }

                    vertex = center + delta;
                }

                if (hasOffset)
                {
                    vertex.x += state.PositionOffset.x;
                    vertex.y += state.PositionOffset.y;
                }

                dstVerts[vertexIndex + i] = vertex;

                Color32 color = srcColors[vertexIndex + i];
                dstColors[vertexIndex + i] = hasTint
                    ? new Color32(
                        (byte)((color.r * tintR + 127) / 255),
                        (byte)((color.g * tintG + 127) / 255),
                        (byte)((color.b * tintB + 127) / 255),
                        (byte)((color.a * tintA + 127) / 255))
                    : color;
            }
        }

        private struct MeshBuffers
        {
            public int VertexIndex;
            public Vector3[] SourceVertices;
            public Color32[] SourceColors;
            public Vector3[] DestinationVertices;
            public Color32[] DestinationColors;
        }
    }
}
