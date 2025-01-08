using System;
using UniRx;
using Zenject;
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

        private readonly IMergeService _mergeService;
        private readonly IGameRuleSettingsRepository _gameRuleSettingsRepository;

        private readonly CompositeDisposable _disposables;

        [Inject]
        public MergeItemUseCase(
            [Inject(Id = "MaxItemNo")] int maxItemNo,
            IMergeService mergeService,
            IGameRuleSettingsRepository gameRuleSettingsRepository)
        {
            if (maxItemNo <= 0)
                throw new ArgumentException("MaxItemNo must be greater than zero.", nameof(maxItemNo));

            _entities = new Dictionary<Guid, IMergeItemEntity>();
            _nextItemIndex = new ReactiveProperty<int>();
            _disposables = new CompositeDisposable();

            MaxItemNo = maxItemNo;
            _mergeService = mergeService ?? throw new ArgumentNullException(nameof(mergeService));
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
            if (itemNo < 0)
                throw new ArgumentException("ItemNo must be non-negative.", nameof(itemNo));

            var entity = new MergeItemEntity(itemNo, _contactTimeLimit);
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
            if (!_entities.TryGetValue(id, out var entity))
            {
                throw new KeyNotFoundException("Entity is not found.");
            }

            entity.AddContactTime(deltaTime);
        }

        public bool CheckGameOver(Guid id)
        {
            if (!_entities.TryGetValue(id, out var entity))
            {
                throw new KeyNotFoundException("Entity is not found.");
            }

            return entity.CheckGameOver();
        }


        public void ResetContactTime(Guid id)
        {
            if (_entities.TryGetValue(id, out var entity))
            {
                entity.ResetContactTime();
            }
        }

        public bool CanMerge(Guid sourceId, Guid targetId)
        {
            if (!_entities.TryGetValue(sourceId, out var source) || !_entities.TryGetValue(targetId, out var target))
            {
                throw new KeyNotFoundException("One or both entities not found.");
            }

            return source.CanMergeWith(target);
        }

        public MergeData CreateMergeData(Guid sourceId, Guid targetId)
        {
            if (!_entities.TryGetValue(sourceId, out var source) || !_entities.TryGetValue(targetId, out var target))
            {
                throw new KeyNotFoundException("One or both entities not found.");
            }

            if (!source.CanMergeWith(target))
            {
                throw new InvalidOperationException("Entities cannot be merged.");
            }

            return _mergeService.CreateMergeData(source.Position, target.Position, source.ItemNo);
        }

        public void UpdateNextItemIndex()
        {
            try
            {
                _nextItemIndex.Value = _mergeService.GenerateNextItemIndex(MaxItemNo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update next item index.", ex);
            }
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
