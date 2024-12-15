using System;

namespace WatermelonGameClone.Domain
{
    public class ScoreResetService : IScoreResetService
    {
        // Determine whether to reset the daily score
        public bool ShouldResetDailyScores(DateTime lastPlayedDate, DateTime currentDate)
        {
            if (lastPlayedDate.Date == null || currentDate.Date == null)
            {
                throw new DomainException("Invalid date provided for daily score reset check.");
            }

            return lastPlayedDate.Date != currentDate.Date;
        }

        // Determine whether to reset the monthly score
        public bool ShouldResetMonthlyScores(DateTime lastPlayedDate, DateTime currentDate)
        {
            if (lastPlayedDate.Month < 1 || lastPlayedDate.Month > 12 || currentDate.Month < 1 || currentDate.Month > 12)
            {
                throw new DomainException("Invalid month provided for monthly score reset check.");
            }

            return lastPlayedDate.Month != currentDate.Month || lastPlayedDate.Year != currentDate.Year;
        }

        // Reset the scores
        public void ResetScores(ref int[] scores)
        {
            if (scores == null)
            {
                throw new DomainException("Scores array cannot be null during reset.");
            }

            scores = new int[0];
        }
    }
}
