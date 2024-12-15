using System;

namespace WatermelonGameClone.Domain
{
    public class MergeItemIndexService : IMergeItemIndexService
    {
        private readonly Random _random;

        public MergeItemIndexService()
        {
            // Random generation with reproducible by specifying seeds
            _random = new Random();
        }

        public int GenerateNextItemIndex(int maxSphereNo)
        {
            if (maxSphereNo <= 0)
            {
                throw new DomainException("Max sphere number must be greater than zero.");
            }
            
            int maxIndex = maxSphereNo / 2 - 1;

            // C# Random.next is required for MaxExclusive, so+1
            return _random.Next(0, maxIndex + 1);
        }
    }
}
