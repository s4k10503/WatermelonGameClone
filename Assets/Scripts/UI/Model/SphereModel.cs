using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class SphereModel : ISphereModel, IDisposable
    {
        private ReactiveProperty<int> _nextSphereIndex = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> NextSphereIndex => _nextSphereIndex.ToReadOnlyReactiveProperty();

        private CompositeDisposable _disposables;


        public int MaxSphereNo { get; private set; }

        [Inject]
        public SphereModel(
            [Inject(Id = "MaxSphereNo")] int maxSphereNo)
        {
            MaxSphereNo = maxSphereNo;
            _disposables = new CompositeDisposable();

            // Adding ReactiveProperties to _disposables ensures it gets disposed
            // when ScoreModel is disposed, preventing memory leaks.
            _nextSphereIndex.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void UpdateNextSphereIndex()
        {
            int maxIndex = MaxSphereNo / 2 - 1;
            _nextSphereIndex.Value = UnityEngine.Random.Range(0, maxIndex + 1);
        }
    }
}