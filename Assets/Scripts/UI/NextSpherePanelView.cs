using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class NextSpherePanelView : MonoBehaviour, INextSpherePanelView
    {
        private GameObject[] _nextSphereImages;
        private GameObject[] _instantiatedSpheres;
        private IUIAnimator _uiAnimator;

        [Inject]
        public void Construct(
            IUIAnimator uiAnimator,
            [Inject(Id = "NextSphereImages")] GameObject[] nextSphereImages)
        {
            _uiAnimator = uiAnimator;
            _uiAnimator.GetUIPosition(this.transform);
            _nextSphereImages = nextSphereImages;

            CreateNextSphereImages();
        }

        private void Update()
        {
            _uiAnimator.HarmonicMotion(this.transform, HarmonicMotionType.Sin);
        }

        private void OnDestroy()
        {

        }

        public void CreateNextSphereImages()
        {
            _instantiatedSpheres = new GameObject[_nextSphereImages.Length];
            for (int i = 0; i < _nextSphereImages.Length; i++)
            {
                GameObject sphere = Instantiate(_nextSphereImages[i], transform);
                sphere.SetActive(false);
                _instantiatedSpheres[i] = sphere;
            }
        }

        public void UpdateNextSphereImages(int sphereIndex)
        {
            for (int i = 0; i < _instantiatedSpheres.Length; i++)
            {
                _instantiatedSpheres[i].SetActive(i == sphereIndex);
            }
        }
    }
}