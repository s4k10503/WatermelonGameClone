using WatermelonGameClone.Domain;
using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    public class MainSceneViewStateData
    {
        public int CurrentScore { get; }
        public int BestScore { get; }
        public ScoreContainer ScoreContainer { get; }
        public RenderTexture Screenshot { get; }

        public MainSceneViewStateData(
            int currentScore, int bestScore, ScoreContainer scoreContainer, RenderTexture screenshot)
        {
            CurrentScore = currentScore;
            BestScore = bestScore;
            ScoreContainer = scoreContainer;
            Screenshot = screenshot;
        }
    }
}
