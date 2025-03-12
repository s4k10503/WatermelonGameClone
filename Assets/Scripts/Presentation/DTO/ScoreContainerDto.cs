using System.Collections.Generic;

namespace Presentation.DTO
{
    public class ScoreContainerDto
    {
        public class RankingData
        {
            public class RankingScores
            {
                public List<int> scores { get; set; }
            }

            public class Rankings
            {
                public RankingScores daily { get; set; }
                public RankingScores monthly { get; set; }
                public RankingScores allTime { get; set; }
            }

            public Rankings rankings { get; set; }
        }

        public RankingData data { get; set; }
    }
}