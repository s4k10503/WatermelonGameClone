using UnityEngine;

namespace WatermelonGameClone.Domain
{
    public class MergeData
    {
        public Vector2 Position;
        public int ItemNo;

        public MergeData(Vector2 position, int itemNo)
        {
            Position = position;
            ItemNo = itemNo;
        }
    }
}