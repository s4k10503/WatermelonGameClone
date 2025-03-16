using Presentation.Interfaces;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Presentation.View.MainScene
{
    public sealed class MergeItemManager : MonoBehaviour, IMergeItemManager
    {
        private Transform _itemPosition;
        private GameObject[] _itemPrefabs;

        private DiContainer _container;

        private readonly Subject<IMergeItemView> _onItemCreated
            = new Subject<IMergeItemView>();
        public IObservable<IMergeItemView> OnItemCreated
            => _onItemCreated;

        [Inject]
        public void Construct(
            DiContainer container,
            [Inject(Id = "ItemPosition")] Transform itemPosition,
            [Inject(Id = "ItemPrefabs")] GameObject[] itemPrefabs)
        {
            _itemPosition = itemPosition;
            _itemPrefabs = itemPrefabs;
            _container = container;

            _onItemCreated.AddTo(this);
        }

        void OnDestroy()
        {
            _itemPosition = null;
            _itemPrefabs = null;
            _container = null;
        }

        public async UniTask CreateItemAsync(Guid id, int itemNo, float delaySeconds, CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);

                GameObject itemObj = _container.InstantiatePrefab(_itemPrefabs[itemNo], _itemPosition);
                IMergeItemView itemView = itemObj.GetComponent<IMergeItemView>();

                itemView.Initialize(id, itemNo);
                itemView.GameObject.SetActive(true);

                _onItemCreated.OnNext(itemView);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error during create item", ex);
            }
        }

        public void MergeItem(Guid id, Vector3 position, int itemNo)
        {
            if (_itemPrefabs == null || itemNo >= _itemPrefabs.Length)
            {
                // Reached maximum itemNo. No further merging possible.
                return;
            }

            GameObject itemObj = _container.InstantiatePrefab(
                _itemPrefabs[itemNo], position, Quaternion.identity, _itemPosition);

            var itemView = itemObj.GetComponent<IMergeItemView>();
            itemView.Initialize(id, itemNo, isAfterMerge: true);
            itemView.GameObject.SetActive(true);
            _onItemCreated.OnNext(itemView);
        }

        public void DestroyItem(GameObject itemObj)
        {
            Destroy(itemObj);
            itemObj = null;
        }
    }
}
