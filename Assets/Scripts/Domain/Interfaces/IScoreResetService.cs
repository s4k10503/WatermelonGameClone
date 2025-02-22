using System;

namespace Domain.Interfaces
{
    public interface IScoreResetService
    {
        bool ShouldResetDailyScores(DateTime lastPlayedDate, DateTime currentDate);
        bool ShouldResetMonthlyScores(DateTime lastPlayedDate, DateTime currentDate);
        void ResetScores(ref int[] scores);
    }
}
