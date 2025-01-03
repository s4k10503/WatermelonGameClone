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

        public MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo)
        {

            if (itemNo < 0)
            {
                throw new DomainException("Item number must be greater than zero.");
            }

            var newPosition = CalculateMergePosition(sourcePosition, targetPosition);

            return new MergeData(newPosition, itemNo);
        }

        private Vector3 CalculateMergePosition(Vector2 position1, Vector2 position2)
        {
            return (position1 + position2) / 2;
        }
    }
}
