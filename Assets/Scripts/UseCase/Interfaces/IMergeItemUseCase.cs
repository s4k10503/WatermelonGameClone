using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
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


        bool CanMerge(int currentItemNo, int targetItemNo);
        MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo);
        void UpdateNextItemIndex();
        bool CheckGameOver(float contactTime);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        void SetNextItemIndex(int index); // Debug only method
#endif
    }
}
