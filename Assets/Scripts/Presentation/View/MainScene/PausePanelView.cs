using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public sealed class PausePanelView : MonoBehaviour, IPausePanelView
    {
        [SerializeField] Button _buttonBackToTitle;
        [SerializeField] Button _buttonRestart;
        [SerializeField] Button _buttonBackToGame;
        [SerializeField] Canvas _canvas;

        public IObservable<Unit> OnRestart
            => _buttonRestart.OnClickAsObservable();
        public IObservable<Unit> OnBackToTitle
            => _buttonBackToTitle.OnClickAsObservable();
        public IObservable<Unit> OnBackToGame
            => _buttonBackToGame.OnClickAsObservable();

        private IUIAnimator _uiAnimator;
        private Vector3 _originalScale;
        private Vector3 _pressedScale;


        [Inject]
        public void Construct(IUIAnimator uiAnimator)
        {
            _originalScale = _buttonBackToTitle.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
            _uiAnimator = uiAnimator;
        }

        private void Start()
        {
            SetupButtonAnimations(_buttonBackToGame);
            SetupButtonAnimations(_buttonBackToTitle);
            SetupButtonAnimations(_buttonRestart);
        }

        public void ShowPanel()
        {
            _canvas.enabled = true;
            transform.SetAsLastSibling();

            _uiAnimator
                .AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }

        public void HidePanel() => _canvas.enabled = false;

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
