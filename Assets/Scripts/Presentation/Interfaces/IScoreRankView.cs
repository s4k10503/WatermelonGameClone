using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public interface IScoreRankView
    {
        // Methods related to UI updates
        void DisplayCurrentScore(int currentScore);
        void DisplayTopScores(ScoreContainer scoreContainer);
    }
}
