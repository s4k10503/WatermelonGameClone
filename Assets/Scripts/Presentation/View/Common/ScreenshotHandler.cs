using UnityEngine;
using UnityEngine.Rendering;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public sealed class ScreenshotHandler : MonoBehaviour, IScreenshotHandler
    {
        private Camera _mainCamera;
        private Camera _uiCamera;
        private RenderTexture _currentRenderTexture;

        [Inject]
        public void Construct(
                [Inject(Id = "Main Camera")] Camera mainCamera,
                [Inject(Id = "UI Camera")] Camera uiCamera)
        {
            _mainCamera = mainCamera;
            _uiCamera = uiCamera;
        }

        void OnDestroy()
        {
            if (_currentRenderTexture != null)
            {
                RenderTexture.ReleaseTemporary(_currentRenderTexture);
                _currentRenderTexture = null;
            }
        }

        public async UniTask<RenderTexture> CaptureScreenshotAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            int width = Screen.width;
            int height = Screen.height;

            _currentRenderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            var oldTargetCamera = _mainCamera.targetTexture;
            var oldTargetUICamera = _uiCamera.targetTexture;

            _mainCamera.targetTexture = _currentRenderTexture;
            _mainCamera.Render();

            // Because if you don't restore the target texture, it may affect the next render
            _mainCamera.targetTexture = oldTargetCamera;

            // Avoid clearing the RenderTexture contents before the UI camera starts rendering.
            // Keep the RenderTexture containing the output of the main camera as is, 
            // and add the contents of the UI camera without overwriting it.
            _uiCamera.clearFlags = CameraClearFlags.Nothing;

            _uiCamera.targetTexture = _currentRenderTexture;
            _uiCamera.Render();
            _uiCamera.targetTexture = oldTargetUICamera;

            var request = AsyncGPUReadback.Request(_currentRenderTexture);
            await UniTask.WaitUntil(() => request.done, cancellationToken: ct);

            if (request.hasError)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError("An error occurred in AsyncGPUReadback.");
#endif

                RenderTexture.ReleaseTemporary(_currentRenderTexture);
                _currentRenderTexture = null;
                return null;
            }

            return _currentRenderTexture;
        }
    }
}
