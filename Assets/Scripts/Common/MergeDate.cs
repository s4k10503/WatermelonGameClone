using UnityEngine;
using System.Collections.Generic;

namespace WatermelonGameClone
{
    public class MergeData
    {
        public Vector3 Position;
        public int SphereNo;
        public GameObject SphereA;
        public GameObject SphereB;

        public MergeData(Vector3 position, int sphereNo, GameObject sphereA, GameObject sphereB)
        {
            Position = position;
            SphereNo = sphereNo;
            SphereA = sphereA;
            SphereB = sphereB;
        }
    }
}