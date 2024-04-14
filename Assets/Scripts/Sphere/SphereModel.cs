using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public class SphereModel : MonoBehaviour
    {
        [SerializeField] private SphereView[] _spherePrefab;
        [SerializeField] private Transform _spherePosition;

        private ReactiveProperty<int> _nextSphereIndex = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> NextSphereIndex => _nextSphereIndex.ToReadOnlyReactiveProperty();

        private int _maxSphereNo;


        public void Initialise()
        {
            _maxSphereNo = _spherePrefab.Length;
            UpdateNextSphereIndex();
        }

        public void UpdateNextSphereIndex()
        {
            int maxIndex = _maxSphereNo / 2 - 1;
            _nextSphereIndex.Value = Random.Range(0, maxIndex + 1);
        }

        public SphereView CreateSphere()
        {
            SphereView sphere = Instantiate(_spherePrefab[_nextSphereIndex.Value], _spherePosition);
            sphere.Initialize(_maxSphereNo, _nextSphereIndex.Value);
            sphere.gameObject.SetActive(true);
            return sphere;
        }

        public void MergeSphere(Vector3 position, int sphereNo)
        {
            SphereView sphere = Instantiate(_spherePrefab[sphereNo + 1], position, Quaternion.identity, _spherePosition);
            sphere.InitializeAfterMerge(_maxSphereNo, sphereNo + 1);
            sphere.GetComponent<Rigidbody2D>().simulated = true;
            sphere.gameObject.SetActive(true);
        }
    }
}