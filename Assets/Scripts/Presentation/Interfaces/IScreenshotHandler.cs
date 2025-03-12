using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Presentation.Interfaces
{
    public interface IScreenshotHandler
    {
        UniTask<RenderTexture> CaptureScreenshotAsync(CancellationToken ct);
    }
}
