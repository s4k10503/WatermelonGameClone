using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    public interface IScreenshotHandler
    {
        UniTask<RenderTexture> CaptureScreenshotAsync(CancellationToken ct);
    }
}
