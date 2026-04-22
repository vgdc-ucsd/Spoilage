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

            _previewReducer.Play(source);
            _previewReducer.Skip();

            _totalChars = _previewReducer.RevealedCount;
            _target.text = TMPFormatter.Build(_previewReducer.DisplayBuffer, _previewReducer.StyleRuns);
            _target.ForceMeshUpdate();

            MarkLayoutDirty();
            SnapshotBaseline();
            _pushedStaticFrame = false;
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
                _target.text = "";
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

            if (_reducer.RegionVersion != _lastRegionVersion)
            {
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

            bool isStatic = (revealed >= _totalChars) && !_anyAnim;
            if (isStatic && _pushedStaticFrame)
            {
                return;
            }

            TMP_TextInfo textInfo = _target.textInfo;
            int charCount = textInfo.characterCount;
            float now = (float)_reducer.TimeSincePlay;
            int regionCount = regions.Count;

            for (int i = 0; i < charCount; i++)
            {
                TMP_CharacterInfo ci = textInfo.characterInfo[i];
                if (!ci.isVisible)
                {
                    continue;
                }

                int matIdx = ci.materialReferenceIndex;
                int vertIdx = ci.vertexIndex;
                if (matIdx < 0 || matIdx >= textInfo.meshInfo.Length)
                {
                    continue;
                }

                Vector3[] srcVerts = _baselineVerts[matIdx];
                Color32[] srcColors = _baselineColors[matIdx];
                Vector3[] dstVerts = textInfo.meshInfo[matIdx].vertices;
                Color32[] dstColors = textInfo.meshInfo[matIdx].colors32;
                if (srcVerts == null || srcColors == null || dstVerts == null || dstColors == null)
                {
                    continue;
                }

                if (vertIdx < 0 ||
                    vertIdx + 3 >= srcVerts.Length ||
                    vertIdx + 3 >= srcColors.Length ||
                    vertIdx + 3 >= dstVerts.Length ||
                    vertIdx + 3 >= dstColors.Length)
                {
                    continue;
                }

                if (i >= revealed)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        dstVerts[vertIdx + k] = srcVerts[vertIdx + k];
                        Color32 c = srcColors[vertIdx + k];
                        dstColors[vertIdx + k] = new Color32(c.r, c.g, c.b, 0);
                    }
                    continue;
                }

                CharAnimState state = CharAnimState.Identity;
                bool charAnim = false;
                for (int r = 0; r < regionCount; r++)
                {
                    Region reg = regions[r];
                    IAnimation animation = reg.Animation;
                    if (animation == null)
                    {
                        continue;
                    }

                    int charIndexInRegion = i - reg.Start;
                    if ((uint)charIndexInRegion >= (uint)reg.Length)
                    {
                        continue;
                    }

                    animation.Apply(ref state, new AnimationContext
                    {
                        CharIndexInBuffer = i,
                        CharIndexInRegion = charIndexInRegion,
                        RegionLength = reg.Length,
                        TimeSinceRegionStart = now - (float)reg.StartTime,
                    });
                    charAnim = true;
                }

                if (charAnim)
                {
                    ApplyStateToQuad(state, srcVerts, srcColors, dstVerts, dstColors, vertIdx);
                }
                else
                {
                    for (int k = 0; k < 4; k++)
                    {
                        dstVerts[vertIdx + k] = srcVerts[vertIdx + k];
                        dstColors[vertIdx + k] = srcColors[vertIdx + k];
                    }
                }

                for (int r = 0; r < regionCount; r++)
                {
                    Region reg = regions[r];
                    if (!(reg.Animation is IVertexAnimation va))
                    {
                        continue;
                    }

                    int charIndexInRegion = i - reg.Start;
                    if ((uint)charIndexInRegion >= (uint)reg.Length)
                    {
                        continue;
                    }

                    for (int k = 0; k < 4; k++)
                    {
                        _quadVerts[k] = dstVerts[vertIdx + k];
                        _quadColors[k] = dstColors[vertIdx + k];
                    }
                    va.ApplyVertices(_quadVerts, _quadColors, new AnimationContext
                    {
                        CharIndexInBuffer = i,
                        CharIndexInRegion = charIndexInRegion,
                        RegionLength = reg.Length,
                        TimeSinceRegionStart = now - (float)reg.StartTime,
                    });
                    for (int k = 0; k < 4; k++)
                    {
                        dstVerts[vertIdx + k] = _quadVerts[k];
                        dstColors[vertIdx + k] = _quadColors[k];
                    }
                }
            }

            _target.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
            _pushedStaticFrame = isStatic;
        }

        void EnsureBaseline()
        {
            if (_baselineValid && !_target.havePropertiesChanged)
            {
                return;
            }

            _target.ForceMeshUpdate();
            SnapshotBaseline();
            _pushedStaticFrame = false;
        }

        void SnapshotBaseline()
        {
            TMP_TextInfo textInfo = _target.textInfo;
            int sub = textInfo.meshInfo == null ? 0 : textInfo.meshInfo.Length;

            if (_baselineVerts == null || _baselineVerts.Length != sub)
            {
                _baselineVerts = new Vector3[sub][];
                _baselineColors = new Color32[sub][];
            }
            for (int m = 0; m < sub; m++)
            {
                TMP_MeshInfo mi = textInfo.meshInfo[m];
                int vn = mi.vertices == null ? 0 : mi.vertices.Length;
                if (_baselineVerts[m] == null || _baselineVerts[m].Length != vn)
                {
                    _baselineVerts[m] = new Vector3[vn];
                }
                if (_baselineColors[m] == null || _baselineColors[m].Length != vn)
                {
                    _baselineColors[m] = new Color32[vn];
                }
                if (vn > 0)
                {
                    System.Array.Copy(mi.vertices, _baselineVerts[m], vn);
                    System.Array.Copy(mi.colors32, _baselineColors[m], vn);
                }
            }
            _baselineValid = true;
        }

        static void ApplyStateToQuad(
            CharAnimState state,
            Vector3[] srcVerts, Color32[] srcColors,
            Vector3[] dstVerts, Color32[] dstColors,
            int i0)
        {
            Vector3 center = (srcVerts[i0] + srcVerts[i0 + 1] + srcVerts[i0 + 2] + srcVerts[i0 + 3]) * 0.25f;

            bool hasRotation = state.Rotation != 0f;
            float cos = 1f, sin = 0f;
            if (hasRotation)
            {
                float rad = state.Rotation * Mathf.Deg2Rad;
                cos = Mathf.Cos(rad);
                sin = Mathf.Sin(rad);
            }
            bool hasScale = state.Scale.x != 1f || state.Scale.y != 1f;
            bool hasOffset = state.PositionOffset.x != 0f || state.PositionOffset.y != 0f;
            bool hasTint = state.ColorTint.r != 1f || state.ColorTint.g != 1f
                          || state.ColorTint.b != 1f || state.ColorTint.a != 1f;

            int tr = 255, tg = 255, tb = 255, ta = 255;
            if (hasTint)
            {
                tr = (int)(state.ColorTint.r * 255f + 0.5f);
                tg = (int)(state.ColorTint.g * 255f + 0.5f);
                tb = (int)(state.ColorTint.b * 255f + 0.5f);
                ta = (int)(state.ColorTint.a * 255f + 0.5f);
            }

            for (int k = 0; k < 4; k++)
            {
                Vector3 v = srcVerts[i0 + k];
                if (hasScale || hasRotation)
                {
                    Vector3 d = v - center;
                    if (hasScale)
                    {
                        d.x *= state.Scale.x;
                        d.y *= state.Scale.y;
                    }
                    if (hasRotation)
                    {
                        float dx = d.x * cos - d.y * sin;
                        float dy = d.x * sin + d.y * cos;
                        d.x = dx; d.y = dy;
                    }
                    v = center + d;
                }
                if (hasOffset)
                {
                    v.x += state.PositionOffset.x;
                    v.y += state.PositionOffset.y;
                }
                dstVerts[i0 + k] = v;

                Color32 c = srcColors[i0 + k];
                dstColors[i0 + k] = hasTint
                    ? new Color32(
                        (byte)((c.r * tr + 127) / 255),
                        (byte)((c.g * tg + 127) / 255),
                        (byte)((c.b * tb + 127) / 255),
                        (byte)((c.a * ta + 127) / 255))
                    : c;
            }
        }
    }
}
