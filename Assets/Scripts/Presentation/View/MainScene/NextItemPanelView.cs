using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public sealed class NextItemPanelView : MonoBehaviour, INextItemPanelView
    {
        private GameObject[] _nextItemImages;
        private GameObject[] _instantiatedItems;
        private IUIAnimator _uiAnimator;

        [Inject]
        public void Construct(
            IUIAnimator uiAnimator,
            [Inject(Id = "NextItemImages")] GameObject[] nextItemImages)
        {
            _uiAnimator = uiAnimator;
            _uiAnimator.GetUIPosition(transform);
            _nextItemImages = nextItemImages;

            CreateNextItemImages();
            this.UpdateAsObservable()
                .Subscribe(_ => _uiAnimator.HarmonicMotion(transform, HarmonicMotionType.Cos))
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _nextItemImages = null;
            _instantiatedItems = null;

            _uiAnimator = null;
        }

        public void CreateNextItemImages()
        {
            _instantiatedItems = new GameObject[_nextItemImages.Length];
            for (int i = 0; i < _nextItemImages.Length; i++)
            {
                GameObject Item = Instantiate(_nextItemImages[i], transform);
                Item.SetActive(false);
                _instantiatedItems[i] = Item;
            }
        }

        public void UpdateNextItemImages(int ItemIndex)
        {
            if (_instantiatedItems == null)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError("someObject is null");
#endif
                return;
            }

            for (int i = 0; i < _instantiatedItems.Length; i++)
            {
                _instantiatedItems[i].SetActive(i == ItemIndex);
            }
        }
    }
}