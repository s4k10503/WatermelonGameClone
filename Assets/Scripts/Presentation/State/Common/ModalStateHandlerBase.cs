namespace WatermelonGameClone.Presentation
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Threading;

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
