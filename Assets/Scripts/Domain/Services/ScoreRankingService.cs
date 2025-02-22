using Domain.Interfaces;

using System.Linq;

namespace Domain.Services
{
    public sealed class ScoreRankingService : IScoreRankingService
    {
        // Update score ranking
        public int[] UpdateTopScores(int[] currentScores, int newScore, int maxEntries)
        {
            if (currentScores == null)
            {
                throw new DomainException("Current scores array cannot be null.");
            }

            if (maxEntries <= 0)
            {
                throw new DomainException("Maximum entries must be greater than zero.");
            }

            var updatedScores = currentScores.ToList();
            updatedScores.Add(newScore);

            return updatedScores.OrderByDescending(x => x).Take(maxEntries).ToArray();
        }

        // Check if the new score is the best score
        public bool IsNewBestScore(int currentBest, int newScore)
        {
            if (newScore < 0)
            {
                throw new DomainException("New score cannot be negative.");
            }
            
            return newScore > currentBest;
        }
    }
}
