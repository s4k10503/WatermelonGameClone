using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface ITitleSceneView
    {
        IObservable<Unit> GameStartRequested { get; }
    }
}
