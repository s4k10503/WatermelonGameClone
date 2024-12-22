using UnityEngine;
using TMPro;
using System.Collections.Generic;
using WatermelonGameClone.Domain;
using UnityEngine.UI;
using System;
using UniRx;
using Zenject;
using UniRx.Triggers;
using DG.Tweening;
using System.Text;

namespace WatermelonGameClone.Presentation
{
    public class LicenseModalView : MonoBehaviour, ILicenseModalView
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _buttonBack;
        [SerializeField] private TextMeshProUGUI _licenseText;

        private IUIAnimator _uiAnimator;
        private Vector3 _originalScale;
        private Vector3 _pressedScale;

        public IObservable<Unit> OnBack
            => _buttonBack.OnClickAsObservable();

        [Inject]
        public void Construct(IUIAnimator uiAnimator)
        {
            _uiAnimator = uiAnimator;
            _originalScale = _buttonBack.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
            _licenseText.text = string.Empty;
        }

        private void Start()
        {
            HideModal();
            SetupButtonAnimations(_buttonBack);
        }

        private void OnDestroy()
        {
            _canvas = null;
            _buttonBack = null;
            _licenseText = null;
            _uiAnimator = null;
        }

        public void ShowModal()
            => _canvas.enabled = true;

        public void HideModal()
            => _canvas.enabled = false;

        public void DisplayLicenses(IReadOnlyList<License> licenses)
        {
            var sb = new StringBuilder();
            foreach (var license in licenses)
            {
                sb.AppendLine($"{license.Name}\n");
                sb.AppendLine($"{license.Type}\n");
                sb.AppendLine($"{license.Copyright}\n");
                sb.AppendLine(string.Join("\n", license.Terms));
                sb.AppendLine("\n");
            }
            _licenseText.text = sb.ToString();
        }

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
