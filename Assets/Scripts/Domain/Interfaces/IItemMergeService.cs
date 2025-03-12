using UnityEngine;

namespace Domain.Interfaces
{
    public interface IItemMergeService
    {
        bool CanMerge(int itemNo);
        bool ShouldMerge(GameObject itemA, GameObject itemB);
    }
}
