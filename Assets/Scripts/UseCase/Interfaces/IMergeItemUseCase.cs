using UseCase.DTO;

using System;
using System.Collections.Generic;
using System.Numerics;
using UniRx;

namespace UseCase.Interfaces
{
    public interface IMergeItemUseCase
    {
        IReadOnlyReactiveProperty<int> NextItemIndex { get; }
        int MaxItemNo { get; }

        MergeItemDto CreateMergeItemDTO(int itemNo);
        MergeItemDto GetMergeItemDTOById(Guid id);
        IReadOnlyList<MergeItemDto> GetAllMergeItemDTOs();

        void AddContactTime(Guid id, float deltaTime);
        void ResetContactTime(Guid id);
        bool CheckGameOver(Guid id);
        bool CanMerge(Guid sourceId, Guid targetId);
        MergeResultDto CreateMergeDataDTO(Guid sourceId, Vector2 sourcePos, Guid targetId, Vector2 targetPos);
        void UpdateNextItemIndex();

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        void SetNextItemIndex(int index);
#endif
    }
}
