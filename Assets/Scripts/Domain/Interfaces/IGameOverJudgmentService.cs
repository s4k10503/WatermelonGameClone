namespace WatermelonGameClone.Domain
{
    public interface IGameOverJudgmentService
    {
        bool CheckGameOver(float contactTime, float timeLimit);
    }
}
