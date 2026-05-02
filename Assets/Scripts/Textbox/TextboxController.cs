using System;
using TMPro;
using UnityEngine;

namespace TextboxControl
{
    /// <summary>
    /// Main script for controlling textbox animations. Place this on a textbox
    /// with a TMP_text child and provide a source string to animate that child.
    /// </summary>
    public class TextboxController : MonoBehaviour
    {
        private TMP_Text _target;
        private Reducer _reducer;
        private TextAnimator _animator;
        private bool _initialized;

        /// <summary>
        /// Fired when the currently playing textbox stream reaches completion.
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// True while the reducer is actively revealing text.
        /// </summary>
        public bool IsRevealing => _initialized && _reducer.IsPlaying;

        private void Awake()
        {
            _target = GetComponentInChildren<TMP_Text>();
            if (_target == null)
            {
                Debug.LogError("[TextboxControl] TextboxController containing object must have contain a TextMeshPro child!", this);
                enabled = false;
                return;
            }

            _reducer = new Reducer();
            _reducer.OnError += msg => Debug.LogWarning($"[TextboxControl] {msg}", this);
            _reducer.OnComplete += () => OnComplete?.Invoke();

            _animator = new TextAnimator(_target, _reducer);
            _initialized = true;
        }

        /// <summary>
        /// Starts a new textbox string from the beginning.
        /// </summary>
        public void Play(string source)
        {
            if (!IsReady())
            {
                return;
            }

            _reducer.Play(source);
            _animator.Prepare(source);
        }

        /// <summary>
        /// Immediately reveals the remaining text for the active text.
        /// </summary>
        public void Skip()
        {
            if (!IsReady())
            {
                return;
            }

            _reducer.Skip();
            _animator.Render();
        }

        /// <summary>
        /// Rebuilds cached animation baselines after external TMP layout-affecting changes.
        /// </summary>
        public void NotifyTextLayoutChanged(bool renderNow = true)
        {
            if (!IsReady())
            {
                return;
            }

            _target.ForceMeshUpdate();
            _animator.MarkLayoutDirty();
            if (renderNow)
            {
                _animator.Render();
            }
        }

        private void LateUpdate()
        {
            if (!IsReady())
            {
                return;
            }

            _reducer.Tick(Time.deltaTime);
            _animator.Render();
        }

        private bool IsReady()
        {
            return _initialized;
        }
    }
}
