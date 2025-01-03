using System;
using UniRx;
using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public interface IMergeItemView
    {
        IObservable<Unit> OnDropping { get; }
        IObservable<(IMergeItemView Source, IMergeItemView Target)> OnMergeRequest { get; }
        IObservable<(Guid id, float deltaTime)> OnContactTimeUpdated { get; }
        IObservable<Guid> OnContactExited { get; }

        GameObject GameObject { get; }
        int ItemNo { get; }

        void Initialize(Guid id, int itemNo, bool isAfterMerge = false);

    }
}
