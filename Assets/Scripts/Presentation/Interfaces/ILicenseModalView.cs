using WatermelonGameClone.Domain;
using System.Collections.Generic;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace WatermelonGameClone.Presentation
{
    public interface ILicenseModalView
    {
        IObservable<Unit> OnBack { get; }

        // Methods related to game state and UI updates
        public void ShowModal();
        public void HideModal();
        UniTask SetLicensesAsync(IReadOnlyList<License> licenses, CancellationToken ct);
        void ForceMeshUpdateText();
    }
}
