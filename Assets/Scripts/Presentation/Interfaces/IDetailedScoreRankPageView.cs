using Domain.ValueObject;

using System;
using UniRx;

namespace Presentation.Interfaces
{
    public interface IDetailedScoreRankPageView
    {
        IObservable<Unit> OnBack { get; }

        void DisplayTopScores(ScoreContainer scoreContainer);
        void ShowPage();
        void HidePage();
    }
}
