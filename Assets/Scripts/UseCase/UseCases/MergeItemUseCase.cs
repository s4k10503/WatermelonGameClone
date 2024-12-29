using System;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public sealed class MergeItemUseCase : IMergeItemUseCase, IDisposable
    {
        public int MaxItemNo { get; private set; }
        private readonly float _contactTimeLimit;
        private readonly ReactiveProperty<int> _nextItemIndex;
        public IReadOnlyReactiveProperty<int> NextItemIndex
            => _nextItemIndex.ToReadOnlyReactiveProperty();

        private readonly IMergeItemIndexService _mergeItemIndexService;
        private readonly IGameOverJudgmentService _gameOverJudgmentService;
        private readonly IGameRuleSettingsRepository _gameRuleSettingsRepository;

        private readonly CompositeDisposable _disposables;

        [Inject]
        public MergeItemUseCase(
            [Inject(Id = "MaxItemNo")] int maxItemNo,
            IMergeItemIndexService itemIndexService,
            IGameOverJudgmentService gameOverJudgmentService, 
            IGameRuleSettingsRepository gameRuleSettingsRepository)
        {
            if (maxItemNo <= 0)
            {
                throw new ArgumentException("MaxItemNo must be greater than zero.", nameof(maxItemNo));
            }

            MaxItemNo = maxItemNo;
            _disposables = new CompositeDisposable();
            _nextItemIndex = new ReactiveProperty<int>();
            _mergeItemIndexService = itemIndexService ?? throw new ArgumentNullException(nameof(itemIndexService));
            _gameOverJudgmentService = gameOverJudgmentService ?? throw new ArgumentNullException(nameof(gameOverJudgmentService));
            _gameRuleSettingsRepository = gameRuleSettingsRepository ?? throw new ArgumentNullException(nameof(_gameRuleSettingsRepository));
            _contactTimeLimit = _gameRuleSettingsRepository.GetContactTimeLimit();

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

        public bool CheckGameOver(float contactTime)
        {
            return _gameOverJudgmentService.CheckGameOver(contactTime, _contactTimeLimit);
        }
    }
}
