namespace Domain.Interfaces
{
    public interface IScoreRankingService
    {
        int[] UpdateTopScores(int[] currentScores, int newScore, int maxEntries);
        bool IsNewBestScore(int currentBest, int newScore);
    }
}
