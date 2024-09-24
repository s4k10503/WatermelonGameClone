using UniRx;
using System.Linq;

namespace WatermelonGameClone.Domain
{
    public class ScoreRankingService : IScoreRankingService
    {
        // Update score ranking
        public int[] UpdateTopScores(int[] currentScores, int newScore, int maxEntries)
        {
            var updatedScores = currentScores.ToList();
            updatedScores.Add(newScore);
            return updatedScores.OrderByDescending(x => x).Take(maxEntries).ToArray();
        }

        // Check if the new score is the best score
        public bool IsNewBestScore(int currentBest, int newScore)
        {
            return newScore > currentBest;
        }
    }
}
