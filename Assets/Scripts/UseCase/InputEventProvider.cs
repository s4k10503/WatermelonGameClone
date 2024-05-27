using UnityEngine;
using System;
using UniRx;

namespace WatermelonGameClone
{
    public class InputEventProvider : MonoBehaviour, IInputEventProvider
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


        void Start()
        {
            _onMouseMove.AddTo(this);
            _onMouseClick.AddTo(this);
            _onRightKey.AddTo(this);
            _onLeftKey.AddTo(this);
            _onEscapeKey.AddTo(this);
        }

        void Update()
        {
            _onMouseMove.OnNext(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (Input.GetMouseButtonDown(0))
            {
                _onMouseClick.OnNext(Unit.Default);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                _onLeftKey.OnNext(Unit.Default);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                _onRightKey.OnNext(Unit.Default);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _onEscapeKey.OnNext(Unit.Default);
            }
        }
    }
}
