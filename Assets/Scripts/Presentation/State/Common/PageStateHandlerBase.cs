namespace WatermelonGameClone.Presentation
{
    using Cysharp.Threading.Tasks;
    using System.Threading;

    public abstract class PageStateHandlerBase<TView, TData>
        : IPageStateHandler<TView, TData>
    {
        public virtual async UniTask ApplyStateAsync(
            TView view, TData data, CancellationToken ct)
        {
            await ResetAllPageAsync(view, ct);
            await ApplyPageAsync(view, data, ct);
        }

        protected abstract UniTask ResetAllPageAsync(TView view, CancellationToken ct);
        protected abstract UniTask ApplyPageAsync(TView view, TData data, CancellationToken ct);
    }
}
