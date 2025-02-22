using Domain.Interfaces;
using Infrastructure.Services;
using Infrastructure.SODefinitions;

using System;
using Zenject;

namespace Infrastructure.Repositories
{
    public class ScoreTableRepository : IScoreTableRepository
    {
        private readonly ScoreTableSettings _scoreTableSettings;

        [Inject]
        public ScoreTableRepository(ScoreTableSettings scoreTableSettings)
        {
            _scoreTableSettings = scoreTableSettings ?? throw new ArgumentNullException(nameof(scoreTableSettings));

            if (_scoreTableSettings.scores == null || _scoreTableSettings.scores.Length == 0)
            {
                throw new InfrastructureException("Score table settings must contain a valid score array.");
            }
        }

        public int[] GetScoreTable()
        {
            return _scoreTableSettings.scores;
        }
    }
}
