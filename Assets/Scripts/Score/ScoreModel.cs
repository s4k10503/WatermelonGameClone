using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;

namespace WatermelonGameClone
{
    public class ScoreModel : IScoreModel
    {
        public List<int> TodayTopScores { get; private set; }

        private DateTime _lastPlayedDate;
        private static readonly int s_scoreCoefficient = 10;


        // ReactiveProperties
        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<int> _bestScore = new ReactiveProperty<int>(0);

        public IReadOnlyReactiveProperty<int> CurrentScore => _currentScore.ToReadOnlyReactiveProperty();
        public IReadOnlyReactiveProperty<int> BestScore => _bestScore.ToReadOnlyReactiveProperty();


        public ScoreModel()
        {
            TodayTopScores = new List<int>();
            LoadScoresFromJson();
            CheckDateAndResetIfNecessary();
        }

        private void OnDestroy()
        {
            _currentScore.Dispose();
            _bestScore.Dispose();
        }

        public void UpdateCurrentScore(int SphereNo)
        {
            int scoreToAdd = (SphereNo + 1) * s_scoreCoefficient;
            _currentScore.Value += scoreToAdd;
        }

        public void UpdateTodayTopScores(int newScore)
        {
            TodayTopScores.Add(newScore);
            TodayTopScores = TodayTopScores.OrderByDescending(x => x).Take(3).ToList();
            UpdateBestScore(newScore);
        }

        private void UpdateBestScore(int newScore)
        {
            if (newScore > _bestScore.Value)
            {
                _bestScore.Value = newScore;
            }
        }

        public void ResetTodayScores()
        {
            TodayTopScores.Clear();
            SaveScoresToJson();
        }

        private void CheckDateAndResetIfNecessary()
        {
            if (_lastPlayedDate.Date != DateTime.Today)
            {
                ResetTodayScores(); // Reset today's scores if date has changed
            }
        }

        public void SaveScoresToJson()
        {
            try
            {
                ScoreContainer scoreContainer = new ScoreContainer
                {
                    CurrentScore = _currentScore.Value,
                    TodayTopScores = TodayTopScores.ToArray(),
                    BestScore = _bestScore.Value,
                    LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                };

                string json = JsonUtility.ToJson(scoreContainer);

                // Save files in the application's persistent data folder
                string path = Path.Combine(Application.persistentDataPath, "score.json");
                File.WriteAllText(path, json);
                _lastPlayedDate = DateTime.Today;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save score to JSON: {e.Message}");
            }
        }

        public ScoreContainer LoadScoresFromJson()
        {
            try
            {
                // Path of the file where the score data was saved
                string path = Path.Combine(Application.persistentDataPath, "score.json");

                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);

                    // Deserialize ScoreContainer object from JSON string
                    ScoreContainer loadedScoreContainer = JsonUtility.FromJson<ScoreContainer>(json);

                    TodayTopScores = new List<int>(loadedScoreContainer.TodayTopScores);
                    _bestScore.Value = loadedScoreContainer.BestScore;
                    _lastPlayedDate = DateTime.Parse(loadedScoreContainer.LastPlayedDate);

                    return loadedScoreContainer;
                }
                else
                {
                    // If the file does not exist, return ScoreContainer with default values
                    return new ScoreContainer
                    {
                        CurrentScore = 0,
                        TodayTopScores = new int[0],
                        BestScore = _bestScore.Value,
                        LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                    };
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load score from JSON: {e.Message}");
                return new ScoreContainer
                {
                    CurrentScore = 0,
                    TodayTopScores = new int[0],
                    BestScore = 0,
                    LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                };
            }
        }

    }
}
