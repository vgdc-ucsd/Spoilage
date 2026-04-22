using System;
using TMPro;
using UnityEngine;

namespace TextboxControl
{
    public class TextboxController : MonoBehaviour
    {
        public event Action OnComplete;

        public bool IsRevealing => _initialized && _reducer.IsPlaying;

        private TMP_Text _target;
        private Reducer _reducer;
        private TextAnimator _animator;
        private bool _initialized;

        void Awake()
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

        public void Play(string source)
        {
            if (!_initialized)
            {
                return;
            }

            _reducer.Play(source);
            _animator.Prepare(source);
        }

        public void Skip()
        {
            if (!_initialized)
            {
                return;
            }

            _reducer.Skip();
            _animator.Render();
        }

        public void NotifyTextLayoutChanged(bool renderNow = true)
        {
            if (!_initialized)
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

        void LateUpdate()
        {
            _reducer.Tick(Time.deltaTime);
            _animator.Render();
        }
    }
}
