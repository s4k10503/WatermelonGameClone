using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public sealed class TitlePanelView : MonoBehaviour, ITitlePanelView
    {
        [SerializeField] Button _buttonGameStart;
        [SerializeField] Button _buttonMyScore;
        [SerializeField] Button _buttonSettings;
        [SerializeField] Button _buttonLicense;
        [SerializeField] Canvas _canvas;

        private IUIAnimator _uiAnimator;
        private Vector3 _originalScale;
        private Vector3 _pressedScale;

        public IObservable<Unit> OnGameStart => _buttonGameStart.OnClickAsObservable();
        public IObservable<Unit> OnMyScore => _buttonMyScore.OnClickAsObservable();
        public IObservable<Unit> OnSettings => _buttonSettings.OnClickAsObservable();
        public IObservable<Unit> OnLicense => _buttonLicense.OnClickAsObservable();

        [Inject]
        public void Construct(IUIAnimator uiAnimator)
        {
            _uiAnimator = uiAnimator;
            _uiAnimator.GetUIPosition(transform);

            _originalScale = _buttonGameStart.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
        }

        private void Start()
        {
            ShowPanel();
            SetupButtonAnimations(_buttonGameStart);
            SetupButtonAnimations(_buttonMyScore);
            SetupButtonAnimations(_buttonSettings);
            SetupButtonAnimations(_buttonLicense);

            this.UpdateAsObservable()
                .Subscribe(_ => _uiAnimator.HarmonicMotion(transform, HarmonicMotionType.Sin))
                .AddTo(this);
        }

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
