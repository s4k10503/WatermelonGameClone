using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;
using UniRx.Triggers;
using DG.Tweening;

namespace WatermelonGameClone.Presentation
{
    public sealed class UserNameModalView : MonoBehaviour, IUserNameModalView
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_InputField _userNameInputField;
        [SerializeField] private Button _buttonSubmit;

        private IUIAnimator _uiAnimator;

        private bool _isVisible = false;

        private Vector3 _centerPosition; // Modal center position
        private Vector2 _offscreenPosition; // Modal offscreen position
        private Vector3 _buttonOriginalScale;
        private Vector3 _buttonPressedScale;

        public IObservable<string> OnUserNameSubmit
            => _buttonSubmit.OnClickAsObservable()
                .Where(_ => !string.IsNullOrEmpty(_userNameInputField.text))
                .Select(_ => _userNameInputField.text);

        [Inject]
        public void Construct(IUIAnimator uiAnimator)
        {
            _canvas.enabled = false;

            // Cache the position of the modal
            var rectTransform = GetComponent<RectTransform>();
            _offscreenPosition = rectTransform.localPosition;
            _centerPosition = Vector2.zero;

            // Set the scales for enlarged and reduced button
            _buttonOriginalScale = _buttonSubmit.transform.localScale;
            _buttonPressedScale = _buttonOriginalScale * 0.9f;

            _uiAnimator = uiAnimator;

            SetupButtonAnimations(_buttonSubmit);
        }

        private void OnDestroy()
        {
            _canvas = null;
            _buttonSubmit = null;
            _userNameInputField = null;
            _uiAnimator = null;
        }

        public void ShowModal()
        {
            if (_isVisible) return;

            _isVisible = true;
            _canvas.enabled = true;

            _uiAnimator.AnimateLocalPosition(
                transform, _centerPosition, 0.25f, Ease.OutBack);
        }

        public void HideModal()
        {
            if (!_isVisible) return;

            _isVisible = false;

            // Move from the center to the outside (top) from the center by animation
            _uiAnimator.AnimateLocalPosition(
                transform, _offscreenPosition, 0.25f, Ease.InBack, onComplete: () => _canvas.enabled = false);
        }

        private void SetupButtonAnimations(Button button)
        {
            // Processing when the button is pressed
            button.OnPointerDownAsObservable()
                .Subscribe(_ =>
                {
                    _uiAnimator
                        .AnimateScale(
                            button.gameObject, _buttonOriginalScale, _buttonPressedScale, 0.1f, Ease.OutQuad);
                })
                .AddTo(this);

            // Processing when the button is released
            button.OnPointerUpAsObservable()
                .Subscribe(_ =>
                {
                    _uiAnimator
                        .AnimateScale(
                            button.gameObject, _buttonPressedScale, _buttonOriginalScale, 0.1f, Ease.OutQuad);
                })
                .AddTo(this);
        }
    }
}
