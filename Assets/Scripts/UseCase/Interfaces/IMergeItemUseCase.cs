using System;
using System.Collections.Generic;
using System.Numerics;
using UniRx;

namespace WatermelonGameClone.UseCase
{
    public interface IMergeItemUseCase
    {
        IReadOnlyReactiveProperty<int> NextItemIndex { get; }
        int MaxItemNo { get; }

        MergeItemDTO CreateMergeItemDTO(int itemNo);
        MergeItemDTO GetMergeItemDTOById(Guid id);
        IReadOnlyList<MergeItemDTO> GetAllMergeItemDTOs();

        void AddContactTime(Guid id, float deltaTime);
        void ResetContactTime(Guid id);
        bool CheckGameOver(Guid id);
        bool CanMerge(Guid sourceId, Guid targetId);
        MergeResultDTO CreateMergeDataDTO(Guid sourceId, Vector2 sourcePos, Guid targetId, Vector2 targetPos);
        void UpdateNextItemIndex();

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        void SetNextItemIndex(int index);
#endif
    }
}
