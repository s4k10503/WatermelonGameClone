using System;
using UniRx;
using UnityEngine;

namespace WatermelonGameClone
{
    public interface ITitlePanelView
    {
        IObservable<Unit> OnGameStart { get; }
    }
}