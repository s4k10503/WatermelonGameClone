using UnityEngine;

namespace WatermelonGameClone.Domain
{
    public interface IMergeJudgmentService
    {
        bool CanMerge(int currentItemNo, int targetItemNo);
        Vector3 CalculateMergePosition(Vector3 position1, Vector3 position2);
    }
}