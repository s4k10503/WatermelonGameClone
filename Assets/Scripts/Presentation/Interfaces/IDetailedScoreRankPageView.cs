using System;
using UniRx;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public interface IDetailedScoreRankPageView
    {
        IObservable<Unit> OnBack { get; }

        void DisplayTopScores(ScoreContainer scoreContainer);
        void ShowPanel();
        void HidePanel();
    }
}
