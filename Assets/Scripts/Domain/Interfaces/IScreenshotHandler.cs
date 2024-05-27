using UnityEngine;
using UnityEngine.Rendering;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface IScreenshotHandler
{
    UniTask<RenderTexture> CaptureScreenshotAsync(CancellationToken ct);
}
