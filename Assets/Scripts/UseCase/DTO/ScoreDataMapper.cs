using Domain.ValueObject;
using UseCase.DTO;
using System.Linq;

namespace UseCase.DTO
{
    /// <summary>
    /// Domain層のValueObjectとUseCase層のDTOを相互変換するマッパー
    /// </summary>
    public static class ScoreDataMapper
    {
        /// <summary>
        /// Domain層のScoreContainerをUseCase層のScoreDataDtoに変換
        /// </summary>
        /// <param name="scoreContainer">Domain層のScoreContainer</param>
        /// <returns>UseCase層のScoreDataDto</returns>
        public static ScoreDataDto? ToScoreDataDto(ScoreContainer scoreContainer)
        {
            if (scoreContainer?.data == null) return null;

            var scoreDto = new ScoreDto(
                scoreContainer.data.score.userName,
                scoreContainer.data.score.best,
                scoreContainer.data.score.lastPlayedDate
            );

            var dailyDto = new RankingCategoryDto(scoreContainer.data.rankings.daily.scores);
            var monthlyDto = new RankingCategoryDto(scoreContainer.data.rankings.monthly.scores);
            var allTimeDto = new RankingCategoryDto(scoreContainer.data.rankings.allTime.scores);

            var rankingsDto = new RankingsDto(dailyDto, monthlyDto, allTimeDto);

            return new ScoreDataDto(scoreDto, rankingsDto);
        }

        /// <summary>
        /// UseCase層のScoreDataDtoをDomain層のScoreContainerに変換
        /// （将来的に必要な場合のために用意）
        /// </summary>
        /// <param name="scoreDataDto">UseCase層のScoreDataDto（nullable）</param>
        /// <returns>Domain層のScoreContainer</returns>
        public static ScoreContainer ToScoreContainer(ScoreDataDto? scoreDataDto)
        {
            if (!scoreDataDto.HasValue) return null;

            // この実装は実際のScoreContainerの構造に応じて調整が必要
            // 現在は概念的な実装として記載
            throw new System.NotImplementedException("ToScoreContainer is not implemented yet");
        }
    }
}