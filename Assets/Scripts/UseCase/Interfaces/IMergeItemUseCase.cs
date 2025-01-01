using UniRx;
using UnityEngine;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public interface IMergeItemUseCase
    {
        public IReadOnlyReactiveProperty<int> NextItemIndex { get; }
        public int MaxItemNo { get; }
        bool CanMerge(int currentItemNo, int targetItemNo);
        MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo);
        void UpdateNextItemIndex();
        bool CheckGameOver(float contactTime);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        void SetNextItemIndex(int index); // Debug only method
#endif
    }
}
