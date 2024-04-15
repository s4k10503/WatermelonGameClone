using UnityEngine;
using System;
using UniRx;

namespace WatermelonGameClone
{
    public class InputEventProvider : MonoBehaviour, IInputEventProvider
    {
        private readonly Subject<Vector2> _onMouseMove = new Subject<Vector2>();
        private readonly Subject<Unit> _onMouseClick = new Subject<Unit>();

        public IObservable<Vector2> OnMouseMove => _onMouseMove;
        public IObservable<Unit> OnMouseClick => _onMouseClick;

        void Start()
        {
            _onMouseMove.AddTo(this);
            _onMouseClick.AddTo(this);
        }

        void Update()
        {
            _onMouseMove.OnNext(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (Input.GetMouseButtonDown(0))
            {
                _onMouseClick.OnNext(Unit.Default);
            }
        }
    }
}
