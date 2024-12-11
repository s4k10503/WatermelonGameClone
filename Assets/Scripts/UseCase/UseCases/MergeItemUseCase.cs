using System;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public sealed class MergeItemUseCase : IMergeItemUseCase, IDisposable
    {
        private readonly ReactiveProperty<int> _nextItemIndex;
        public IReadOnlyReactiveProperty<int> NextItemIndex
            => _nextItemIndex.ToReadOnlyReactiveProperty();

        private readonly IMergeItemIndexService _mergeItemIndexService;

        private readonly CompositeDisposable _disposables;

        public int MaxItemNo { get; private set; }

        [Inject]
        public MergeItemUseCase(
            [Inject(Id = "MaxItemNo")] int maxItemNo,
            IMergeItemIndexService itemIndexService)
        {
            if (maxItemNo <= 0)
            {
                throw new ArgumentException("MaxItemNo must be greater than zero.", nameof(maxItemNo));
            }

            _disposables = new CompositeDisposable();
            _nextItemIndex = new ReactiveProperty<int>();
            _mergeItemIndexService = itemIndexService ?? throw new ArgumentNullException(nameof(itemIndexService));
            MaxItemNo = maxItemNo;
            _nextItemIndex.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public void UpdateNextItemIndex()
        {
            try
            {
                _nextItemIndex.Value = _mergeItemIndexService.GenerateNextItemIndex(MaxItemNo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update next item index.", ex);
            }
        }
    }
}