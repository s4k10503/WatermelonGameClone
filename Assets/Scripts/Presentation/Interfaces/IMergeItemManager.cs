using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Presentation.Interfaces
{
    public interface IMergeItemManager
    {
        IObservable<IMergeItemView> OnItemCreated { get; }

        UniTask CreateItemAsync(Guid id, int itemNo, float delaySeconds, CancellationToken ct);
        void MergeItem(Guid id, Vector3 position, int itemNo);
        void DestroyItem(GameObject itemView);
    }
}
