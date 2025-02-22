using Presentation.Interfaces;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Presentation.State.Common
{
    public abstract class ModalStateHandlerBase<TView, TData>
        : IModalStateHandler<TView, TData>, IDisposable
    {
        public virtual async UniTask ApplyStateAsync(
            TView view, TData data, CancellationToken ct)
        {
            await ResetAllModalAsync(view, ct);
            await ApplyModalAsync(view, data, ct);
        }

        public virtual void Dispose()
        {
        }

        protected abstract UniTask ResetAllModalAsync(TView view, CancellationToken ct);

        // Actual modal display processing is implemented at the inheritance destination
        protected abstract UniTask ApplyModalAsync(TView view, TData data, CancellationToken ct);
    }
}
