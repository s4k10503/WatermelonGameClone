using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace WatermelonGameClone.Presentation
{
    public sealed class InputEventProvider : MonoBehaviour, IInputEventProvider
    {
        private readonly Subject<Vector2> _mouseMoveSubject = new Subject<Vector2>();
        private readonly Subject<Unit> _mouseClickSubject = new Subject<Unit>();
        private readonly Subject<Unit> _leftKeySubject = new Subject<Unit>();
        private readonly Subject<Unit> _rightKeySubject = new Subject<Unit>();
        private readonly Subject<Unit> _escapeKeySubject = new Subject<Unit>();

        public IObservable<Vector2> OnMouseMove => _mouseMoveSubject;
        public IObservable<Unit> OnMouseClick => _mouseClickSubject;
        public IObservable<Unit> OnLeftKey => _leftKeySubject;
        public IObservable<Unit> OnRightKey => _rightKeySubject;
        public IObservable<Unit> OnEscapeKey => _escapeKeySubject;

        private bool _isInputProcessed;
        private Vector2 _lastMousePosition;

        void Awake()
        {
            _mouseMoveSubject.AddTo(this);
            _mouseClickSubject.AddTo(this);
            _rightKeySubject.AddTo(this);
            _leftKeySubject.AddTo(this);
            _escapeKeySubject.AddTo(this);
            _isInputProcessed = false;
        }

        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed)
                .Select(_ => (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition))
                .Where(pos => pos != _lastMousePosition)
                .Subscribe(HandleMouseMove)
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed && Input.GetMouseButtonDown(0))
                .Subscribe(HandleMouseClick)
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed &&
                            (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)))
                .Subscribe(HandleLeftKey)
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed &&
                            (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)))
                .Subscribe(HandleRightKey)
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed && Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(HandleEscapeKey)
                .AddTo(this);

            // Reset flag
            this.LateUpdateAsObservable()
                .Subscribe(_ => _isInputProcessed = false)
                .AddTo(this);
        }

        private void HandleMouseMove(Vector2 pos)
        {
            _mouseMoveSubject.OnNext(pos);
            _lastMousePosition = pos;
            _isInputProcessed = true;
        }

        private void HandleMouseClick(Unit _)
        {
            _mouseClickSubject.OnNext(Unit.Default);
            _isInputProcessed = true;
        }

        private void HandleLeftKey(Unit _)
        {
            _leftKeySubject.OnNext(Unit.Default);
            _isInputProcessed = true;
        }

        private void HandleRightKey(Unit _)
        {
            _rightKeySubject.OnNext(Unit.Default);
            _isInputProcessed = true;
        }

        private void HandleEscapeKey(Unit _)
        {
            _escapeKeySubject.OnNext(Unit.Default);
            _isInputProcessed = true;
        }
    }
}
