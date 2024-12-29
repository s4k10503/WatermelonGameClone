using UniRx;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public interface IMergeItemUseCase
    {
        public IReadOnlyReactiveProperty<int> NextItemIndex { get; }
        public int MaxItemNo { get; }
        void UpdateNextItemIndex();
        bool CheckGameOver(float contactTime);
    }
}
