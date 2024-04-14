using UnityEngine;

namespace WatermelonGameClone
{
    public interface ISphereModel
    {
        // Methods for sphere control
        void Initialize(int maxSphereNo);
        void UpdateNextSphereIndex();
    }
}
