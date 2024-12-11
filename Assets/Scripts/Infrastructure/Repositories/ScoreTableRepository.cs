using System;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Infrastructure
{
    public class ScoreTableRepository : IScoreTableRepository
    {
        private readonly ScoreTableSettings _scoreTableSettings;

        [Inject]
        public ScoreTableRepository(ScoreTableSettings scoreTableSettings)
        {
            _scoreTableSettings = scoreTableSettings ?? throw new ArgumentNullException(nameof(scoreTableSettings));

            if (_scoreTableSettings.Scores == null || _scoreTableSettings.Scores.Length == 0)
            {
                throw new InfrastructureException("Score table settings must contain a valid score array.");
            }
        }

        public int[] GetScoreTable()
        {
            return _scoreTableSettings.Scores;
        }
    }
}
