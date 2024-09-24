using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace WatermelonGameClone.Presentation
{
    public sealed class InputEventProvider : MonoBehaviour, IInputEventProvider
    {
        private readonly Subject<Vector2> _onMouseMove = new Subject<Vector2>();
        private readonly Subject<Unit> _onMouseClick = new Subject<Unit>();
        private readonly Subject<Unit> _onLeftKey = new Subject<Unit>();
        private readonly Subject<Unit> _onRightKey = new Subject<Unit>();
        private readonly Subject<Unit> _onEscapeKey = new Subject<Unit>();

        public IObservable<Vector2> OnMouseMove => _onMouseMove;
        public IObservable<Unit> OnMouseClick => _onMouseClick;
        public IObservable<Unit> OnLeftKey => _onLeftKey;
        public IObservable<Unit> OnRightKey => _onRightKey;
        public IObservable<Unit> OnEscapeKey => _onEscapeKey;

        private bool _isInputProcessed;
        private Vector2 _lastMousePosition;


        void Awake()
        {
            _onMouseMove.AddTo(this);
            _onMouseClick.AddTo(this);
            _onRightKey.AddTo(this);
            _onLeftKey.AddTo(this);
            _onEscapeKey.AddTo(this);
            _isInputProcessed = false;
        }

        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed)
                .Select(_ => (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition))
                .Where(pos => pos != _lastMousePosition)
                .Subscribe(pos =>
                {
                    _onMouseMove.OnNext(pos);
                    _lastMousePosition = pos;
                    _isInputProcessed = true;
                })
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed &&
                            Input.GetMouseButtonDown(0))
                .Subscribe(_ =>
                {
                    _onMouseClick.OnNext(Unit.Default);
                    _isInputProcessed = true;
                })
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed &&
                            (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)))
                .Subscribe(_ =>
                {
                    _onLeftKey.OnNext(Unit.Default);
                    _isInputProcessed = true;
                })
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed &&
                            (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)))
                .Subscribe(_ =>
                {
                    _onRightKey.OnNext(Unit.Default);
                    _isInputProcessed = true;
                })
                .AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => !_isInputProcessed &&
                            Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ =>
                {
                    _onEscapeKey.OnNext(Unit.Default);
                    _isInputProcessed = true;
                })
                .AddTo(this);

            // Reset flag
            this.LateUpdateAsObservable()
                .Subscribe(_ => _isInputProcessed = false)
                .AddTo(this);
        }
    }
}
