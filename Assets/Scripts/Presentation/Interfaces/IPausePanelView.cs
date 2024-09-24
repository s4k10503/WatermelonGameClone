using System;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    public interface IPausePanelView
    {
        IObservable<Unit> OnRestart { get; }
        IObservable<Unit> OnBackToTitle { get; }
        IObservable<Unit> OnBackToGame { get; }

        void ShowPanel();
        void HidePanel();
    }
}