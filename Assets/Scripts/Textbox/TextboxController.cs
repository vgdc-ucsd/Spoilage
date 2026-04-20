using System;
using TMPro;
using UnityEngine;

namespace TextboxControl
{
    public class TextboxController : MonoBehaviour
    {
        public event Action OnComplete;

        public bool IsRevealing => _reducer != null && _reducer.IsPlaying;

        private TMP_Text _target;
        private Reducer _reducer;
        private int _lastRevealed = -1;
        private int _lastRunCount = -1;

        void Awake()
        {
            _target = GetComponentInChildren<TMP_Text>();
            if (_target == null)
            {
                Debug.LogError("[TextboxControl] TextboxController containing object must have contain a TextMeshPro child!", this);
                return;
            }

            _reducer = new Reducer();
            _reducer.OnError += msg => Debug.LogWarning($"[TextboxControl] {msg}", this);
            _reducer.OnComplete += () => OnComplete?.Invoke();
        }

        public void Play(string source)
        {
            if (_reducer == null || _target == null)
            {
                return;
            }

            _reducer.Play(source);
            _lastRevealed = -1;
            _lastRunCount = -1;
            Render();
        }

        public void Skip()
        {
            if (_reducer == null)
            {
                return;
            }

            _reducer.Skip();
            Render();
        }

        void Update()
        {
            if (_reducer == null)
            {
                return;
            }

            _reducer.Tick(Time.deltaTime);
            Render();
        }

        void Render()
        {
            if (_target == null || _reducer == null)
            {
                return;
            }

            if (_lastRevealed == _reducer.RevealedCount && _lastRunCount == _reducer.StyleRuns.Count)
            {
                return;
            }

            _lastRevealed = _reducer.RevealedCount;
            _lastRunCount = _reducer.StyleRuns.Count;
            _target.text = TMPFormatter.Build(_reducer.DisplayBuffer, _reducer.StyleRuns);
        }
    }
}
