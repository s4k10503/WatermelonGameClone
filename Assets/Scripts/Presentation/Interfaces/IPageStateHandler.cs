using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    public interface IPageStateHandler<TView, TData>
    {
        UniTask ApplyStateAsync(TView view, TData data, CancellationToken ct);
    }

}
