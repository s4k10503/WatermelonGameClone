using UnityEngine;

namespace WatermelonGameClone.Domain
{
    public interface IMergeJudgmentService
    {
        bool CanMerge(int currentItemNo, int targetItemNo);
        MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo);
    }
}