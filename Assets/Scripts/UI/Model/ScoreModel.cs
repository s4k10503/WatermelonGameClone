using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;

namespace WatermelonGameClone
{
    public class ScoreModel : IScoreModel, IDisposable
    {
        private DateTime _lastPlayedDate;
        private static readonly int s_scoreCoefficient = 10;
        private static readonly string s_scoresFilePath = Path.Combine(Application.persistentDataPath, "score.json");

        // ReactiveProperties
        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<int> _bestScore = new ReactiveProperty<int>(0);

        public IReadOnlyReactiveProperty<int> CurrentScore => _currentScore.ToReadOnlyReactiveProperty();
        public IReadOnlyReactiveProperty<int> BestScore => _bestScore.ToReadOnlyReactiveProperty();

        private CompositeDisposable _disposables;
        
        public ScoreContainer ScoreData { get; private set; }

        public ScoreModel()
        {
            ScoreData = new ScoreContainer
            {
                TodayTopScores = new int[0],
                MonthlyTopScores = new int[0],
                AllTimeTopScores = new int[0],
                LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
            };

            _disposables = new CompositeDisposable();

            // Adding ReactiveProperties to _disposables ensures it gets disposed
            // when ScoreModel is disposed, preventing memory leaks.
            _currentScore.AddTo(_disposables);
            _bestScore.AddTo(_disposables);

            LoadScoresFromJson();
            CheckDateAndResetIfNecessary();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void UpdateCurrentScore(int SphereNo)
        {
            int scoreToAdd = (SphereNo + 1) * s_scoreCoefficient;
            _currentScore.Value += scoreToAdd;
        }

        public void UpdateScoreRanking(int newScore)
        {
            UpdateTodayTopScores(newScore);
            UpdateMonthlyTopScores(newScore);
            UpdateAllTimeTopScores(newScore);
            SaveScoresToJson();
        }

        private void UpdateTodayTopScores(int newScore)
        {
            var todayTopScores = ScoreData.TodayTopScores.ToList();
            todayTopScores.Add(newScore);
            ScoreData.TodayTopScores = todayTopScores.OrderByDescending(x => x).Take(3).ToArray();
        }

        private void UpdateMonthlyTopScores(int newScore)
        {
            var monthlyTopScores = ScoreData.MonthlyTopScores.ToList();
            monthlyTopScores.Add(newScore);
            ScoreData.MonthlyTopScores = monthlyTopScores.OrderByDescending(x => x).Take(3).ToArray();
        }

        private void UpdateAllTimeTopScores(int newScore)
        {
            var allTimeTopScores = ScoreData.AllTimeTopScores.ToList();
            allTimeTopScores.Add(newScore);
            ScoreData.AllTimeTopScores = allTimeTopScores.OrderByDescending(x => x).Take(3).ToArray();

            if (newScore > _bestScore.Value)
            {
                _bestScore.Value = newScore;
            }
        }

        private void ResetScores(ref int[] scores)
        {
            scores = new int[0];
            SaveScoresToJson();
        }

        private void CheckDateAndResetIfNecessary()
        {
            DateTime currentDate = DateTime.Today;
            if (_lastPlayedDate.Date != currentDate)
            {
                // Reset today's scores if date has changed
                ResetScores(ref ScoreData.TodayTopScores);
            }

            if (_lastPlayedDate.Month != currentDate.Month ||
                _lastPlayedDate.Year != currentDate.Year)
            {
                // Reset monthly scores if month has changed
                ResetScores(ref ScoreData.MonthlyTopScores);
            }
        }

        public void SaveScoresToJson()
        {
            try
            {
                ScoreData.CurrentScore = _currentScore.Value;
                ScoreData.LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd");

                // Save files in the application's persistent data folder
                File.WriteAllText(s_scoresFilePath, JsonUtility.ToJson(ScoreData));
                _lastPlayedDate = DateTime.Today;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save score to JSON: {e.Message}");
            }
        }

        public void LoadScoresFromJson()
        {
            try
            {
                if (File.Exists(s_scoresFilePath))
                {
                    string json = File.ReadAllText(s_scoresFilePath);

                    // Deserialize ScoreContainer object from JSON string
                    ScoreData = JsonUtility.FromJson<ScoreContainer>(json);

                    _bestScore.Value = ScoreData.AllTimeTopScores.FirstOrDefault();
                    _lastPlayedDate = DateTime.Parse(ScoreData.LastPlayedDate);
                }
                else
                {
                    // If the file does not exist, initialize ScoreContainer with default values
                    ScoreData = new ScoreContainer
                    {
                        CurrentScore = 0,
                        TodayTopScores = new int[0],
                        MonthlyTopScores = new int[0],
                        AllTimeTopScores = new int[0],
                        LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                    };
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load score from JSON: {e.Message}");
                ScoreData = new ScoreContainer
                {
                    CurrentScore = 0,
                    TodayTopScores = new int[0],
                    MonthlyTopScores = new int[0],
                    AllTimeTopScores = new int[0],
                    LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                };
            }
        }
    }
}
