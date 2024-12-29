using UnityEngine;

namespace WatermelonGameClone.Domain
{
    public class MergeJudgmentService : IMergeJudgmentService
    {
        public bool CanMerge(int currentItemNo, int targetItemNo)
        {
            if (currentItemNo < 0 || targetItemNo < 0)
            {
                throw new DomainException("Item numbers must be greater than zero.");
            }

            return currentItemNo == targetItemNo;
        }

        public Vector3 CalculateMergePosition(Vector3 position1, Vector3 position2)
        {
            return (position1 + position2) / 2;
        }
    }
}
