using UnityEngine;

namespace WatermelonGameClone.Domain
{
    public class MergeData
    {
        public Vector3 Position;
        public int ItemNo;
        public GameObject ItemA;
        public GameObject ItemB;

        public MergeData(Vector3 position, int itemNo, GameObject itemA, GameObject itemB)
        {
            Position = position;
            ItemNo = itemNo;
            ItemA = itemA;
            ItemB = itemB;
        }
    }
}