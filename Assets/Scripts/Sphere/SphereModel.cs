using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public class SphereModel : ISphereModel
    {
        private ReactiveProperty<int> _nextSphereIndex = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> NextSphereIndex => _nextSphereIndex.ToReadOnlyReactiveProperty();

        public int MaxSphereNo { get; private set; }

        private void OnDestroy()
        {
            _nextSphereIndex.Dispose();
        }

        public void Initialize(int maxSphereNo)
        {
            MaxSphereNo = maxSphereNo;
        }

        public void UpdateNextSphereIndex()
        {
            int maxIndex = MaxSphereNo / 2 - 1;
            _nextSphereIndex.Value = Random.Range(0, maxIndex + 1);
        }
    }
}