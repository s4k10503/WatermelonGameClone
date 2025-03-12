using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObject;
using UseCase.Interfaces;
using UseCase.DTO;

using System;
using UniRx;
using Zenject;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace UseCases.UseCase
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
            _contactTimeLimit = gameRuleSettingsRepository.GetContactTimeLimit();

            _nextItemIndex.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public MergeItemDto CreateMergeItemDTO(int itemNo)
        {
            var entity = CreateMergeItemEntity(itemNo);
            return MapEntityToDto(entity);
        }

        private IMergeItemEntity CreateMergeItemEntity(int itemNo)
        {
            if (itemNo < 0)
                throw new ArgumentException("ItemNo must be non-negative.", nameof(itemNo));

            var entity = new MergeItemEntity(itemNo, _contactTimeLimit);
            _entities[entity.Id] = entity;
            return entity;
        }

        public MergeItemDto GetMergeItemDTOById(Guid id)
        {
            if (_entities.TryGetValue(id, out var entity))
            {
                return MapEntityToDto(entity);
            }
            return null;
        }

        private MergeItemDto MapEntityToDto(IMergeItemEntity entity)
        {
            return new MergeItemDto
            {
                Id = entity.Id,
                ItemNo = entity.ItemNo,
                Position = entity.Position,
                ContactTime = entity.ContactTime
            };
        }

        public IReadOnlyList<MergeItemDto> GetAllMergeItemDTOs()
        {
            return _entities.Values
                .Select(MapEntityToDto)
                .ToList();
        }

        private void RemoveEntity(Guid id)
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

        public MergeResultDto CreateMergeDataDTO(Guid sourceId, Vector2 sourcePos, Guid targetId, Vector2 targetPos)
        {
            if (!_entities.TryGetValue(sourceId, out var source) || !_entities.TryGetValue(targetId, out var target))
            {
                throw new KeyNotFoundException("One or both entities not found.");
            }

            if (!source.CanMergeWith(target))
            {
                throw new InvalidOperationException("Entities cannot be merged.");
            }

            RemoveEntity(sourceId);
            RemoveEntity(targetId);

            var mergeData = _mergeService.CreateMergeData(sourcePos, targetPos, source.ItemNo);
            return MapMergeDataToDto(mergeData);
        }


        private MergeResultDto MapMergeDataToDto(MergeData mergeData)
        {
            return new MergeResultDto
            {
                Position = mergeData.Position,
                ItemNo = mergeData.ItemNo
            };
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
