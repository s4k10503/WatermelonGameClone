using Domain.ValueObject;
using Presentation.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Presentation.View.Common
{
    public sealed class LicenseModalView : MonoBehaviour, ILicenseModalView
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
            _uiAnimator = uiAnimator ?? throw new ArgumentNullException(nameof(uiAnimator));
            _originalScale = _buttonBack.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
            _licenseText.text = string.Empty;

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

        public async UniTask SetLicensesAsync(IReadOnlyList<License> licenses, CancellationToken ct)
        {
            try
            {
                var sb = new StringBuilder();
                foreach (var license in licenses)
                {
                    sb.AppendLine($"{license.name}\n");
                    sb.AppendLine($"{license.type}\n");
                    sb.AppendLine($"{license.copyright}\n");
                    sb.AppendLine(string.Join("\n", license.terms));
                    sb.AppendLine("\n");

                    // Divide large amounts of data processing and process each frame
                    if (sb.Length <= 500) continue;
                    _licenseText.text += sb.ToString();
                    sb.Clear();
                    await UniTask.Yield(ct);
                }

                // Set the remaining text
                _licenseText.text += sb.ToString();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in SetLicensesAsync.", ex);
            }
        }

        public void ForceMeshUpdateText()
        {
            _licenseText.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();
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
