using System.Numerics;

namespace WatermelonGameClone.Domain
{
    public class MergeService : IMergeService
    {
        private readonly System.Random _random;

        public MergeService()
        {
            // Random generation with reproducible by specifying seeds
            _random = new System.Random();
        }

        public MergeData CreateMergeData(Vector2 sourcePosition, Vector2 targetPosition, int itemNo)
        {

            if (itemNo < 0)
            {
                throw new DomainException("Item number must be greater than zero.");
            }

            var newPosition = CalculateMergePosition(sourcePosition, targetPosition);
            var newItemNo = itemNo + 1;

            return new MergeData(newPosition, newItemNo);
        }

        private Vector2 CalculateMergePosition(Vector2 position1, Vector2 position2)
        {
            return (position1 + position2) / 2;
        }

        public int GenerateNextItemIndex(int maxItemNo)
        {
            if (maxItemNo <= 0)
            {
                throw new DomainException("Max item number must be greater than zero.");
            }

            int maxIndex = maxItemNo / 2 - 1;

            // C# Random.next is required for MaxExclusive, so+1
            return _random.Next(0, maxIndex + 1);
        }
    }
}
