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
        private Transform _ItemPosition;
        private GameObject[] _ItemPrefabs;

        private DiContainer _container;

        public readonly Subject<IMergeItemView> _onItemCreated
            = new Subject<IMergeItemView>();
        public IObservable<IMergeItemView> OnItemCreated
            => _onItemCreated;

        private CancellationTokenSource _cts;

        [Inject]
        public void Construct(
            DiContainer container,
            [Inject(Id = "ItemPosition")] Transform ItemPosition,
            [Inject(Id = "ItemPrefabs")] GameObject[] ItemPrefabs)
        {
            _ItemPosition = ItemPosition;
            _ItemPrefabs = ItemPrefabs;
            _container = container;
        }

        void Awake()
        {
            _cts = new CancellationTokenSource();
            _onItemCreated.AddTo(this);
        }

        void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public async UniTask CreateItem(int ItemNo, float delaySeconds, CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: ct);
            GameObject ItemObj = _container.InstantiatePrefab(_ItemPrefabs[ItemNo], _ItemPosition);
            IMergeItemView ItemView = ItemObj.GetComponent<IMergeItemView>();

            ItemView.Initialize(ItemNo);
            ItemView.GameObject.SetActive(true);
            _onItemCreated.OnNext(ItemView);
        }

        public void MergeItem(Vector3 position, int ItemNo)
        {
            GameObject ItemObj = _container.InstantiatePrefab(
                _ItemPrefabs[ItemNo + 1], position, Quaternion.identity, _ItemPosition);

            var ItemView = ItemObj.GetComponent<IMergeItemView>();
            ItemView.InitializeAfterMerge(ItemNo + 1);
            ((MonoBehaviour)ItemView).GetComponent<Rigidbody2D>().simulated = true;
            ItemView.GameObject.SetActive(true);
            _onItemCreated.OnNext(ItemView);
        }

        public void DestroyItem(GameObject ItemView)
        {
            Destroy(ItemView.gameObject);
        }
    }
}
