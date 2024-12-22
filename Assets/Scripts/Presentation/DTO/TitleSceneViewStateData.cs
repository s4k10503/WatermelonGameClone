using System.Collections.Generic;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public class TitleSceneViewStateData
    {
        public ScoreContainer ScoreContainer { get; }
        public IReadOnlyList<License> Licenses { get; }

        public TitleSceneViewStateData(
            ScoreContainer scoreContainer,
            IReadOnlyList<License> licenses)
        {
            ScoreContainer = scoreContainer;
            Licenses = licenses;
        }
    }
}