using System;
using UniRx;
using Zenject;
using UnityEngine;
using WatermelonGameClone.Domain;
using System.Collections.Generic;
using System.Linq;

namespace WatermelonGameClone.UseCase
{
    public sealed class MergeItemUseCase : IMergeItemUseCase, IDisposable
    {
        public int MaxItemNo { get; private set; }
        private readonly Dictionary<Guid, IMergeItemEntity> _entities;
        private readonly float _contactTimeLimit;
        private readonly ReactiveProperty<int> _nextItemIndex;
        public IReadOnlyReactiveProperty<int> NextItemIndex
            => _nextItemIndex.ToReadOnlyReactiveProperty();

        private readonly IMergeItemIndexService _mergeItemIndexService;
        private readonly IMergeJudgmentService _mergeJudgmentService;
        private readonly IGameOverJudgmentService _gameOverJudgmentService;
        private readonly IGameRuleSettingsRepository _gameRuleSettingsRepository;

        private readonly CompositeDisposable _disposables;

        [Inject]
        public MergeItemUseCase(
            [Inject(Id = "MaxItemNo")] int maxItemNo,
            IMergeItemIndexService itemIndexService,
            IMergeJudgmentService mergeJudgmentService,
            IGameOverJudgmentService gameOverJudgmentService,
            IGameRuleSettingsRepository gameRuleSettingsRepository)
        {
            if (maxItemNo <= 0)
            {
                throw new ArgumentException("MaxItemNo must be greater than zero.", nameof(maxItemNo));
            }

            _entities = new Dictionary<Guid, IMergeItemEntity>();
            _nextItemIndex = new ReactiveProperty<int>();
            _disposables = new CompositeDisposable();

            MaxItemNo = maxItemNo;
            _mergeItemIndexService = itemIndexService ?? throw new ArgumentNullException(nameof(itemIndexService));
            _mergeJudgmentService = mergeJudgmentService ?? throw new ArgumentNullException(nameof(mergeJudgmentService));
            _gameOverJudgmentService = gameOverJudgmentService ?? throw new ArgumentNullException(nameof(gameOverJudgmentService));
            _gameRuleSettingsRepository = gameRuleSettingsRepository ?? throw new ArgumentNullException(nameof(_gameRuleSettingsRepository));
            _contactTimeLimit = _gameRuleSettingsRepository.GetContactTimeLimit();

            _nextItemIndex.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public IMergeItemEntity CreateMergeItemEntity(int itemNo)
        {
            var entity = new MergeItemEntity(itemNo);
            _entities[entity.Id] = entity;
            return entity;
        }

        public IMergeItemEntity GetEntityById(Guid id)
        {
            return _entities.ContainsKey(id) ? _entities[id] : null;
        }

        public IReadOnlyList<IMergeItemEntity> GetAllEntities()
        {
            return _entities.Values.ToList();
        }

        public void RemoveEntity(Guid id)
        {
            _entities.Remove(id);
        }

        public void AddContactTime(Guid id, float deltaTime)
        {
            if (_entities.TryGetValue(id, out var entity))
            {
                entity.AddContactTime(deltaTime);
            }
        }

        public void ResetContactTime(Guid id)
        {
            if (_entities.TryGetValue(id, out var entity))
            {
                entity.ResetContactTime();
            }
        }

        public bool CanMerge(int currentItemNo, int targetItemNo)
        {
            return _mergeJudgmentService.CanMerge(currentItemNo, targetItemNo);
        }

        public MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo)
        {
            return _mergeJudgmentService.CreateMergeData(sourcePosition, targetPosition, itemNo);
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

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public void SetNextItemIndex(int index)
        {
            try
            {
                _nextItemIndex.Value = index; // Set directly for debugging
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to set next item index.", ex);
            }
        }
#endif
    }
}
