using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Presentation.DTO;
using UniRx;

namespace Presentation.Interfaces
{
    public interface ILicenseModalView
    {
        IObservable<Unit> OnBack { get; }

        // Methods related to game state and UI updates
        public void ShowModal();
        public void HideModal();
        UniTask SetLicensesAsync(IReadOnlyList<LicenseDto> licenses, CancellationToken ct);
        void ForceMeshUpdateText();
    }
}
