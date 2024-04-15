using UnityEngine;
using System;
using UniRx;

namespace WatermelonGameClone
{
    public interface ISphereView
    {
        IObservable<Unit> OnGameOver { get; }
        IObservable<Unit> OnDropping { get; }
        IObservable<Unit> OnMoving { get; }
        IObservable<MergeData> OnMerging { get; }

        IReadOnlyReactiveProperty<int> NextSphereIndex { get; }

        void Initialize(int maxSphereNo, int sphereNo);
        void InitializeAfterMerge(int maxSphereNo, int sphereNo);
    }
}
