using System;
using System.Threading;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public sealed class MergeItemManager : MonoBehaviour, IMergeItemManager
    {
        private Transform _itemPosition;
        private GameObject[] _itemPrefabs;

        private DiContainer _container;

        public readonly Subject<IMergeItemView> _onItemCreated
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
        }

        void Awake()
        {
            _onItemCreated.AddTo(this);
        }

        void OnDestroy()
        {
            _itemPosition = null;
            _itemPrefabs = null;
            _container = null;
        }

        public async UniTask CreateItem(int itemNo, float delaySeconds, CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);
            GameObject itemObj = _container.InstantiatePrefab(_itemPrefabs[itemNo], _itemPosition);
            IMergeItemView itemView = itemObj.GetComponent<IMergeItemView>();

            itemView.Initialize(itemNo);
            itemView.GameObject.SetActive(true);
            _onItemCreated.OnNext(itemView);
        }

        public void MergeItem(Vector3 position, int itemNo)
        {
            GameObject itemObj = _container.InstantiatePrefab(
                _itemPrefabs[itemNo + 1], position, Quaternion.identity, _itemPosition);

            var itemView = itemObj.GetComponent<IMergeItemView>();
            itemView.InitializeAfterMerge(itemNo + 1);
            ((MonoBehaviour)itemView).GetComponent<Rigidbody2D>().simulated = true;
            itemView.GameObject.SetActive(true);
            _onItemCreated.OnNext(itemView);
        }

        public void DestroyItem(GameObject itemView)
        {
            Destroy(itemView);
            itemView = null;
        }
    }
}
