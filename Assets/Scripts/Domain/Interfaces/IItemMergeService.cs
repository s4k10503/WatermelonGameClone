
using UnityEngine;

namespace WatermelonGameClone.Domain
{
    public interface IItemMergeService
    {
        bool CanMerge(int itemNo);
        bool ShouldMerge(GameObject itemA, GameObject itemB);
    }
}
