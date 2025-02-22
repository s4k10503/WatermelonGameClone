using System.Threading;
using Cysharp.Threading.Tasks;

namespace Presentation.Interfaces
{
    public interface IModalStateHandler<TView, TData>
    {
        UniTask ApplyStateAsync(TView view, TData data, CancellationToken ct);
    }

}