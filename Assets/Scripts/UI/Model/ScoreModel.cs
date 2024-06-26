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
        public ScoreContainer ScoreData { get; private set; }
        private DateTime _lastPlayedDate;
        private static readonly int s_scoreCoefficient = 10;
        private static readonly string s_scoresFilePath = Path.Combine(Application.persistentDataPath, "score.json");

        // ReactiveProperties
        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<int> _bestScore = new ReactiveProperty<int>(0);

        public IReadOnlyReactiveProperty<int> CurrentScore => _currentScore.ToReadOnlyReactiveProperty();
        public IReadOnlyReactiveProperty<int> BestScore => _bestScore.ToReadOnlyReactiveProperty();

        private CompositeDisposable _disposables;


        public ScoreModel()
        {
            _disposables = new CompositeDisposable();

            // Adding ReactiveProperties to _disposables ensures it gets disposed
            // when ScoreModel is disposed, preventing memory leaks.
            _currentScore.AddTo(_disposables);
            _bestScore.AddTo(_disposables);

            InitializeScoreData();
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
            UpdateDailyTopScores(newScore);
            UpdateMonthlyTopScores(newScore);
            UpdateAllTimeTopScores(newScore);
            SaveScoresToJson();
        }

        private void UpdateDailyTopScores(int newScore)
        {
            var dailyTopScores = ScoreData.Data.Rankings.Daily.Scores.ToList();
            dailyTopScores.Add(newScore);
            ScoreData.Data.Rankings.Daily.Scores = dailyTopScores.OrderByDescending(x => x).Take(7).ToArray();
        }

        private void UpdateMonthlyTopScores(int newScore)
        {
            var monthlyTopScores = ScoreData.Data.Rankings.Monthly.Scores.ToList();
            monthlyTopScores.Add(newScore);
            ScoreData.Data.Rankings.Monthly.Scores = monthlyTopScores.OrderByDescending(x => x).Take(7).ToArray();
        }

        private void UpdateAllTimeTopScores(int newScore)
        {
            var allTimeTopScores = ScoreData.Data.Rankings.AllTime.Scores.ToList();
            allTimeTopScores.Add(newScore);
            ScoreData.Data.Rankings.AllTime.Scores = allTimeTopScores.OrderByDescending(x => x).Take(7).ToArray();

            if (newScore > _bestScore.Value)
            {
                _bestScore.Value = newScore;
                ScoreData.Data.Score.Best = newScore;
            }
        }

        private void ResetScores(ref int[] scores)
        {
            scores = new int[7];
            SaveScoresToJson();
        }

        private void CheckDateAndResetIfNecessary()
        {
            DateTime currentDate = DateTime.Today;
            if (_lastPlayedDate.Date != currentDate)
            {
                // Reset today's scores if date has changed
                ResetScores(ref ScoreData.Data.Rankings.Daily.Scores);
            }

            if (_lastPlayedDate.Month != currentDate.Month ||
                _lastPlayedDate.Year != currentDate.Year)
            {
                // Reset monthly scores if month has changed
                ResetScores(ref ScoreData.Data.Rankings.Monthly.Scores);
            }
        }

        public void SaveScoresToJson()
        {
            try
            {
                ScoreData.Data.Score.Current = _currentScore.Value;
                ScoreData.Data.Score.LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd");

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

                    _bestScore.Value = ScoreData.Data.Score.Best;
                    _lastPlayedDate = DateTime.Parse(ScoreData.Data.Score.LastPlayedDate);
                }
                else
                {
                    // If the file does not exist, initialize ScoreContainer with default values
                    InitializeScoreData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load score from JSON: {e.Message}");
                InitializeScoreData();
            }
        }

        private void InitializeScoreData()
        {
            ScoreData = new ScoreContainer
            {
                Data = new ScoreData
                {
                    Score = new ScoreDetail
                    {
                        Current = 0,
                        Best = 0,
                        LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                    },
                    Rankings = new Rankings
                    {
                        Daily = new ScoreList { Scores = new int[7] },
                        Monthly = new ScoreList { Scores = new int[7] },
                        AllTime = new ScoreList { Scores = new int[7] }
                    }
                }
            };
            _bestScore.Value = 0;
            _lastPlayedDate = DateTime.Today;
        }
    }
}
