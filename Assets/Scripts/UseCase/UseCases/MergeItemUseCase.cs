using System;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public sealed class MergeItemUseCase : IMergeItemUseCase, IDisposable
    {
        private ReactiveProperty<int> _nextItemIndex;
        public IReadOnlyReactiveProperty<int> NextItemIndex
            => _nextItemIndex.ToReadOnlyReactiveProperty();

        private readonly IMergeItemIndexService _mergeItemIndexService;

        private CompositeDisposable _disposables;

        public int MaxItemNo { get; private set; }

        [Inject]
        public MergeItemUseCase(
            [Inject(Id = "MaxItemNo")] int maxItemNo,
            IMergeItemIndexService itemIndexService)
        {
            _disposables = new CompositeDisposable();
            _nextItemIndex = new ReactiveProperty<int>();
            _mergeItemIndexService = itemIndexService;
            MaxItemNo = maxItemNo;
            _nextItemIndex.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void UpdateNextItemIndex()
        {
            _nextItemIndex.Value = _mergeItemIndexService.GenerateNextItemIndex(MaxItemNo);
        }
    }
}