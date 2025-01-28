using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    public interface IModalStateHandler<TView, TData>
    {
        UniTask ApplyStateAsync(TView view, TData data, CancellationToken ct);
    }

}