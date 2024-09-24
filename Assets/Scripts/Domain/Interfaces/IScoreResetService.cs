using System;

namespace WatermelonGameClone.Domain
{
    public interface IScoreResetService
    {
        bool ShouldResetDailyScores(DateTime lastPlayedDate, DateTime currentDate);
        bool ShouldResetMonthlyScores(DateTime lastPlayedDate, DateTime currentDate);
        void ResetScores(ref int[] scores);
    }
}
