using System;
using System.IO;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Infrastructure
{
    public sealed class JsonScoreRepository : IScoreRepository
    {
        private static readonly string s_scoresFilePath = Path.Combine(Application.persistentDataPath, "score.json");

        public async UniTask SaveScoresAsync(ScoreContainer scoreData, CancellationToken ct)
        {
            try
            {
                string json = JsonUtility.ToJson(scoreData);
                await File.WriteAllTextAsync(s_scoresFilePath, json, ct);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save score to JSON: {e.Message}");
            }
        }

        public async UniTask<ScoreContainer> LoadScoresAsync(CancellationToken ct)
        {
            try
            {
                if (File.Exists(s_scoresFilePath))
                {
                    string json = await File.ReadAllTextAsync(s_scoresFilePath, ct);
                    return JsonUtility.FromJson<ScoreContainer>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load score from JSON: {e.Message}");
            }

            return new ScoreContainer
            {
                Data = new ScoreData
                {
                    Score = new ScoreDetail
                    {
                        Best = 0,
                        LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                    },
                    Rankings = new Rankings
                    {
                        Daily = new ScoreList { Scores = new int[0] },
                        Monthly = new ScoreList { Scores = new int[0] },
                        AllTime = new ScoreList { Scores = new int[0] }
                    }
                }
            };
        }
    }
}
