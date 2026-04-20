using System;
using TMPro;
using UnityEngine;

namespace TextboxControl
{
    public class TextboxController : MonoBehaviour
    {
        public event Action OnComplete;

        public bool IsRevealing => _reducer != null && _reducer.IsPlaying;

        private Reducer _reducer;
        private TextAnimator _animator;

        void Awake()
        {
            TMP_Text target = GetComponentInChildren<TMP_Text>();
            if (target == null)
            {
                Debug.LogError("[TextboxControl] TextboxController containing object must have contain a TextMeshPro child!", this);
                return;
            }

            _reducer = new Reducer();
            _reducer.OnError += msg => Debug.LogWarning($"[TextboxControl] {msg}", this);
            _reducer.OnComplete += () => OnComplete?.Invoke();

            _animator = new TextAnimator(target, _reducer);
        }

        public void Play(string source)
        {
            if (_reducer == null)
            {
                return;
            }

            _reducer.Play(source);
            _animator.Prepare(source);
        }

        public void Skip()
        {
            if (_reducer == null)
            {
                return;
            }

            _reducer.Skip();
            _animator.Render();
        }

        void Update()
        {
            if (_reducer == null)
            {
                return;
            }

            _reducer.Tick(Time.deltaTime);
            _animator.Render();
        }
    }
}
