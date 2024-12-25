using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    public interface ITitleSceneViewStateHandler
    {
        UniTask ApplyAsync(TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct);
    }
}
