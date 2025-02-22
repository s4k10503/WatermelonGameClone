using Domain.ValueObject;

using System.Numerics;

namespace Domain.Interfaces
{
    public interface IMergeService
    {
        MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo);
        int GenerateNextItemIndex(int maxItemNo);
    }
}
