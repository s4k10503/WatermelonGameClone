using Presentation.DTO;

using System;
using UniRx;

namespace Presentation.Interfaces
{
    public interface IDetailedScoreRankPageView
    {
        IObservable<Unit> OnBack { get; }

        void DisplayTopScores(ScoreContainerDto scoreContainer);
        void ShowPage();
        void HidePage();
    }
}
