namespace WatermelonGameClone.Presentation
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Threading;

    public abstract class PageStateHandlerBase<TView, TData>
        : IPageStateHandler<TView, TData>, IDisposable
    {
        public virtual async UniTask ApplyStateAsync(
            TView view, TData data, CancellationToken ct)
        {
            await ResetAllPageAsync(view, ct);
            await ApplyPageAsync(view, data, ct);
        }

        public virtual void Dispose()
        {
        }

        protected abstract UniTask ResetAllPageAsync(TView view, CancellationToken ct);
        protected abstract UniTask ApplyPageAsync(TView view, TData data, CancellationToken ct);
    }
}
