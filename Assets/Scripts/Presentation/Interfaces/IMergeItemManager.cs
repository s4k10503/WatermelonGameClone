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

        UniTask CreateItemAsync(int itemNo, float delaySeconds, CancellationToken ct);
        void MergeItem(Vector3 position, int itemNo);
        void DestroyItem(GameObject itemView);
    }
}
