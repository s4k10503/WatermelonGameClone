using System;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    public interface IPauseModalView
    {
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }
        IObservable<Unit> OnBackToGame { get; }

        void ShowPanel();
        void HidePanel();
    }
}