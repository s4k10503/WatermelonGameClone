using UnityEngine;
using System;
using UniRx;

namespace WatermelonGameClone
{
    public interface ISphereModel
    {
        public IReadOnlyReactiveProperty<int> NextSphereIndex { get; }
        public int MaxSphereNo { get; }

        // Methods for sphere control
        void Initialize(int maxSphereNo);
        void UpdateNextSphereIndex();
    }
}
