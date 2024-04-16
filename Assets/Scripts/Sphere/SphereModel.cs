using UnityEngine;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class SphereModel : ISphereModel
    {
        private ReactiveProperty<int> _nextSphereIndex = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> NextSphereIndex => _nextSphereIndex.ToReadOnlyReactiveProperty();

        public int MaxSphereNo { get; private set; }

        [Inject]
        public SphereModel(
            [Inject(Id = "MaxSphereNo")] int maxSphereNo)
        {
            MaxSphereNo = maxSphereNo;
        }

        private void OnDestroy()
        {
            _nextSphereIndex.Dispose();
        }

        public void UpdateNextSphereIndex()
        {
            int maxIndex = MaxSphereNo / 2 - 1;
            _nextSphereIndex.Value = Random.Range(0, maxIndex + 1);
        }
    }
}