using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public class SettingsPanelView : MonoBehaviour, ISettingsPanelView
    {
        [SerializeField] private Slider _sliderBGM;
        [SerializeField] private Slider _sliderSE;
        [SerializeField] private Button _buttonBack;
        [SerializeField] private Canvas _canvas;

        private IUIAnimator _uiAnimator;
        private Vector3 _originalScale;
        private Vector3 _pressedScale;

        public IReadOnlyReactiveProperty<float> ValueBgm
            => _sliderBGM
                .OnValueChangedAsObservable()
                .ToReactiveProperty(_sliderBGM.value);
        public IReadOnlyReactiveProperty<float> ValueSe
            => _sliderSE
                .OnValueChangedAsObservable()
                .ToReactiveProperty(_sliderSE.value);

        public IObservable<Unit> OnBack
            => _buttonBack.OnClickAsObservable();


        [Inject]
        public void Construct(IUIAnimator uiAnimator)
        {
            _uiAnimator = uiAnimator;
            _originalScale = _buttonBack.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
        }

        private void Start()
        {
            HidePanel();
            SetupButtonAnimations(_buttonBack);
        }

        private void OnDestroy()
        {
            _sliderBGM = null;
            _sliderSE = null;
            _buttonBack = null;
            _canvas = null;
            _uiAnimator = null;
        }

        public void SetBgmSliderValue(float value)
            => _sliderBGM.value = value;

        public void SetSeSliderValue(float value)
            => _sliderSE.value = value;

        public void ShowPanel()
            => _canvas.enabled = true;

        public void HidePanel()
            => _canvas.enabled = false;

        private void SetupButtonAnimations(Button button)
        {
            // Processing when the button is pressed
            button.OnPointerDownAsObservable()
                .Subscribe(_ =>
                {
                    _uiAnimator
                        .AnimateScale(
                            button.gameObject, _originalScale, _pressedScale, 0.1f, Ease.OutQuad);
                })
                .AddTo(this);

            // Processing when the button is released
            button.OnPointerUpAsObservable()
                .Subscribe(_ =>
                {
                    _uiAnimator
                        .AnimateScale(
                            button.gameObject, _pressedScale, _originalScale, 0.1f, Ease.OutQuad);
                })
                .AddTo(this);
        }
    }
}
