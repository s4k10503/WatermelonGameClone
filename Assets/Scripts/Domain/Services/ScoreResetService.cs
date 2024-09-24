using System;

namespace WatermelonGameClone.Domain
{
    public class ScoreResetService : IScoreResetService
    {
        // Determine whether to reset the daily score
        public bool ShouldResetDailyScores(DateTime lastPlayedDate, DateTime currentDate)
        {
            return lastPlayedDate.Date != currentDate;
        }

        // Determine whether to reset monthly score
        public bool ShouldResetMonthlyScores(DateTime lastPlayedDate, DateTime currentDate)
        {
            return lastPlayedDate.Month != currentDate.Month || lastPlayedDate.Year != currentDate.Year;
        }

        // Reset the score
        public void ResetScores(ref int[] scores)
        {
            scores = new int[0];
        }
    }
}
