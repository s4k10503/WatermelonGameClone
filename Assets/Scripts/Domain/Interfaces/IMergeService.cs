using System.Numerics;

namespace WatermelonGameClone.Domain
{
    public interface IMergeService
    {
        MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo);
        int GenerateNextItemIndex(int maxItemNo);
    }
}
