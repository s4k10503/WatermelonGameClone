using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public class TitleSceneViewStateData
    {
        public ScoreContainer ScoreContainer { get; }

        public TitleSceneViewStateData(ScoreContainer scoreContainer)
        {
            ScoreContainer = scoreContainer;
        }
    }
}