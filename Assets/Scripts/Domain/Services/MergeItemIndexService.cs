namespace WatermelonGameClone.Domain
{
    public class MergeItemIndexService : IMergeItemIndexService
    {
        public int GenerateNextItemIndex(int maxSphereNo)
        {
            int maxIndex = maxSphereNo / 2 - 1;
            return UnityEngine.Random.Range(0, maxIndex + 1);
        }
    }
}
