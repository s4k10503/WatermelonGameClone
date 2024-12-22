using WatermelonGameClone.Domain;
using System.Collections.Generic;
using System;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    public interface ILicenseModalView
    {
        IObservable<Unit> OnBack { get; }

        // Methods related to game state and UI updates
        public void ShowModal();
        public void HideModal();
        public void DisplayLicenses(IReadOnlyList<License> licenses);
    }
}
