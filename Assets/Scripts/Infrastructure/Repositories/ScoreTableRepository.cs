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
            _scoreTableSettings = scoreTableSettings;
        }

        public int[] GetScoreTable()
        {
            return _scoreTableSettings.Scores;
        }
    }
}
