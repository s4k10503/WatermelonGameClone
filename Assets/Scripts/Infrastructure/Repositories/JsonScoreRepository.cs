using Domain.Interfaces;
using Domain.ValueObject;
using Infrastructure.Services;

using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Repositories
{
    public sealed class JsonScoreRepository : IScoreRepository
    {
        private static readonly string ScoresFilePath = Path.Combine(Application.persistentDataPath, "score.json");

        public async UniTask SaveScoresAsync(ScoreContainer scoreData, CancellationToken ct)
        {
            if (scoreData == null)
            {
                throw new InfrastructureException("Cannot save null score data.");
            }

            try
            {
                var json = JsonUtility.ToJson(scoreData);
                await File.WriteAllTextAsync(ScoresFilePath, json, ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Failed to save scores to JSON file.", ex);
            }
        }

        public async UniTask<ScoreContainer> LoadScoresAsync(CancellationToken ct)
        {
            try
            {
                // Score file not found. Returning default score data.
                if (!File.Exists(ScoresFilePath)) 
                    return CreateDefaultScoreContainer();
                
                var json = await File.ReadAllTextAsync(ScoresFilePath, ct);
                return JsonUtility.FromJson<ScoreContainer>(json);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new InfrastructureException("Failed to load scores from JSON file.", ex);
            }
        }

        private ScoreContainer CreateDefaultScoreContainer()
        {
            return new ScoreContainer
            {
                data = new ScoreData
                {
                    score = new ScoreDetail
                    {
                        best = 0,
                        lastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                    },
                    rankings = new Rankings
                    {
                        daily = new ScoreList { scores = Array.Empty<int>() },
                        monthly = new ScoreList { scores = Array.Empty<int>() },
                        allTime = new ScoreList { scores = Array.Empty<int>() }
                    }
                }
            };
        }
    }
}
