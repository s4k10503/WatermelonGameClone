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
using Cysharp.Threading.Tasks;
using System.Threading;

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

        public async UniTask SetLicensesAsync(IReadOnlyList<License> licenses, CancellationToken ct)
        {
            var sb = new StringBuilder();
            foreach (var license in licenses)
            {
                sb.AppendLine($"{license.Name}\n");
                sb.AppendLine($"{license.Type}\n");
                sb.AppendLine($"{license.Copyright}\n");
                sb.AppendLine(string.Join("\n", license.Terms));
                sb.AppendLine("\n");

                // Divide large amounts of data processing and process each frame
                if (sb.Length > 500)
                {
                    _licenseText.text += sb.ToString();
                    sb.Clear();
                    await UniTask.Yield(ct);
                }
            }

            // Set the remaining text
            _licenseText.text += sb.ToString();
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
