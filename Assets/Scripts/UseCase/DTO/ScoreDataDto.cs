using System;

namespace UseCase.DTO
{
    /// <summary>
    /// UseCase層用のスコアデータDTO（値型）
    /// </summary>
    public readonly struct ScoreDataDto
    {
        public ScoreDto Score { get; }
        public RankingsDto Rankings { get; }

        public ScoreDataDto(ScoreDto score, RankingsDto rankings)
        {
            Score = score;
            Rankings = rankings;
        }
    }

    /// <summary>
    /// スコア情報DTO（値型）
    /// </summary>
    public readonly struct ScoreDto
    {
        public string UserName { get; }
        public int Best { get; }
        public string LastPlayedDate { get; }

        public ScoreDto(string userName, int best, string lastPlayedDate)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Best = best;
            LastPlayedDate = lastPlayedDate ?? throw new ArgumentNullException(nameof(lastPlayedDate));
        }
    }

    /// <summary>
    /// ランキングデータDTO（値型）
    /// </summary>
    public readonly struct RankingsDto
    {
        public RankingCategoryDto Daily { get; }
        public RankingCategoryDto Monthly { get; }
        public RankingCategoryDto AllTime { get; }

        public RankingsDto(RankingCategoryDto daily, RankingCategoryDto monthly, RankingCategoryDto allTime)
        {
            Daily = daily;
            Monthly = monthly;
            AllTime = allTime;
        }
    }

    /// <summary>
    /// ランキングカテゴリDTO（値型）
    /// </summary>
    public readonly struct RankingCategoryDto
    {
        public int[] Scores { get; }

        public RankingCategoryDto(int[] scores)
        {
            Scores = scores ?? throw new ArgumentNullException(nameof(scores));
        }
    }
}