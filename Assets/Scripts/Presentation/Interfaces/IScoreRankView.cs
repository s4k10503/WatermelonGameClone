using Presentation.DTO;

namespace Presentation.Interfaces
{
    public interface IScoreRankView
    {
        // Methods related to UI updates
        void DisplayCurrentScore(int currentScore);
        void DisplayTopScores(ScoreContainerDto scoreContainer);
    }
}
