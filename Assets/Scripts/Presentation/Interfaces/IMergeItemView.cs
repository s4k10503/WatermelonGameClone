using System;
using UniRx;
using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public interface IMergeItemView
    {
        IObservable<Unit> OnDropping { get; }
        IObservable<(IMergeItemView Source, IMergeItemView Target)> OnMergeRequest { get; }
        IReadOnlyReactiveProperty<int> NextItemIndex { get; }
        IReadOnlyReactiveProperty<float> ContactTime { get; }
        GameObject GameObject { get; }
        int ItemNo { get; }

        void Initialize(int itemNo);
        void InitializeAfterMerge(int itemNo);

    }
}
