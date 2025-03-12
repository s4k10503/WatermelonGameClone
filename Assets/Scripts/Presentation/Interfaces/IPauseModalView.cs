using System;
using UniRx;

namespace Presentation.Interfaces
{
    public interface IPauseModalView
    {
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }
        IObservable<Unit> OnBackToGame { get; }

        void ShowModal();
        void HideModal();
    }
}