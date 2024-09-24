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

        UniTask CreateItem(int sphereNo, float delaySeconds, CancellationToken ct);
        void MergeItem(Vector3 position, int sphereNo);
        void DestroyItem(GameObject sphereView);
    }
}
