using System;
using UniRx;
using UnityEngine;

namespace Presentation.Interfaces
{
    public interface IMergeItemView
    {
        IObservable<Unit> OnDropping { get; }
        IObservable<(IMergeItemView Source, IMergeItemView Target)> OnMergeRequest { get; }
        IObservable<(Guid id, float deltaTime)> OnContactTimeUpdated { get; }
        IObservable<Guid> OnContactExited { get; }

        GameObject GameObject { get; }
        Guid Id { get; }
        int ItemNo { get; }

        void Initialize(Guid id, int itemNo, bool isAfterMerge = false);

    }
}
