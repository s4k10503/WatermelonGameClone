using System;
using UniRx;
using WatermelonGameClone.Domain;
using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public interface IMergeItemView
    {
        IObservable<Unit> OnGameOver { get; }
        IObservable<Unit> OnDropping { get; }
        IObservable<MergeData> OnMerging { get; }
        IReadOnlyReactiveProperty<int> NextSphereIndex { get; }
        GameObject GameObject { get; }
        int SphereNo { get; }

        void Initialize(int sphereNo);
        void InitializeAfterMerge(int sphereNo);

    }
}
