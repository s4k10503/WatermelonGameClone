using System;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    public interface ITitlePanelView
    {
        IObservable<Unit> OnGameStart { get; }
        IObservable<Unit> OnMyScore { get; }
        IObservable<Unit> OnSettings { get; }
        IObservable<Unit> OnLicense { get; }

        void ShowPanel();
        void HidePanel();
    }
}