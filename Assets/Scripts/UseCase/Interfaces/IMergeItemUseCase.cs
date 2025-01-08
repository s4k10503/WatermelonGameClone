using System;
using System.Collections.Generic;
using UniRx;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public interface IMergeItemUseCase
    {
        IReadOnlyReactiveProperty<int> NextItemIndex { get; }
        int MaxItemNo { get; }

        IMergeItemEntity CreateMergeItemEntity(int itemNo);
        IMergeItemEntity GetEntityById(Guid id);
        IReadOnlyList<IMergeItemEntity> GetAllEntities();
        void RemoveEntity(Guid id);
        void AddContactTime(Guid id, float deltaTime);
        void ResetContactTime(Guid id);
        bool CheckGameOver(Guid id);
        bool CanMerge(Guid sourceId, Guid targetId);
        MergeData CreateMergeData(Guid sourceId, Guid targetId);
        void UpdateNextItemIndex();


#if DEVELOPMENT_BUILD || UNITY_EDITOR
        void SetNextItemIndex(int index); // Debug only method
#endif
    }
}
