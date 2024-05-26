using UnityEngine;
using UnityEngine.Rendering;
using System.Threading;
using Cysharp.Threading.Tasks;

public class ScreenshotHandler : MonoBehaviour, IScreenshotHandler
{
    private Camera _camera;
    private RenderTexture _currentRenderTexture;

    void Awake()
    {
        _camera = Camera.main;
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
        var oldTarget = _camera.targetTexture;

        _camera.targetTexture = _currentRenderTexture;
        _camera.Render();
        _camera.targetTexture = oldTarget;

        var request = AsyncGPUReadback.Request(_currentRenderTexture);
        await UniTask.WaitUntil(() => request.done, cancellationToken: ct);

        if (request.hasError)
        {
            Debug.LogError("An error occurred in AsyncGPUReadback.");
            RenderTexture.ReleaseTemporary(_currentRenderTexture);
            _currentRenderTexture = null;
            return null;
        }

        return _currentRenderTexture;
    }
}
