using System;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public interface IMergeItemManager
    {
        IObservable<IMergeItemView> OnItemCreated { get; }

        UniTask CreateItemAsync(Guid id, int itemNo, float delaySeconds, CancellationToken ct);
        void MergeItem(Guid id, Vector3 position, int itemNo);
        void DestroyItem(GameObject itemView);
    }
}
