using System;
using UniRx;

namespace Presentation.Interfaces
{
    public interface ITitlePageView
    {
        IObservable<Unit> OnGameStart { get; }
        IObservable<Unit> OnMyScore { get; }
        IObservable<Unit> OnSettings { get; }
        IObservable<Unit> OnLicense { get; }

        void ShowPage();
        void HidePage();
    }
}