using Domain.ValueObject;

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Presentation.Interfaces
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
