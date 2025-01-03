namespace WatermelonGameClone.Domain
{
    public class GameOverJudgmentService : IGameOverJudgmentService
    {
        public bool CheckGameOver(float contactTime, float timeLimit)
        {
            return contactTime > timeLimit;
        }
    }
}
