using System;
using UniRx;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public interface IDetailedScoreRankView
    {
        IObservable<Unit> OnBack { get; }

        void DisplayTopScores(ScoreContainer scoreContainer);
        void ShowPanel();
        void HidePanel();
    }
}
