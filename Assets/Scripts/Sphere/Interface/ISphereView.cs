using UnityEngine;
using System;
using UniRx;

namespace WatermelonGameClone
{
    public interface ISphereView
    {
        IObservable<Unit> OnGameOverRequest { get; }
        IObservable<Unit> OnDroppingRequest { get; }
        IObservable<Unit> OnMovingRequest { get; }
        IObservable<MergeData> OnMergingRequest { get; }

        IReadOnlyReactiveProperty<int> NextSphereIndex { get; }

        void Initialize(int maxSphereNo, int sphereNo);
        void InitializeAfterMerge(int maxSphereNo, int sphereNo);
    }
}
