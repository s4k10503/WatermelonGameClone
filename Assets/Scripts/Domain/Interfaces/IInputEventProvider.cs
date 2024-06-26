using System;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IInputEventProvider
    {
        // Observables for UI events
        IObservable<Vector2> OnMouseMove { get; }
        IObservable<Unit> OnMouseClick { get; }
        IObservable<Unit> OnLeftKey { get; }
        IObservable<Unit> OnRightKey { get; }
        IObservable<Unit> OnEscapeKey { get; }
    }
}
