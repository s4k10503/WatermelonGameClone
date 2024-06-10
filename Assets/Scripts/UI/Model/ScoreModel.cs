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
        public List<int> TodayTopScores { get; private set; }
        public List<int> MonthlyTopScores { get; private set; }
        public List<int> AllTimeTopScores { get; private set; }

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
            TodayTopScores = new List<int>();
            MonthlyTopScores = new List<int>();
            AllTimeTopScores = new List<int>();

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
            TodayTopScores.Add(newScore);
            TodayTopScores = TodayTopScores.OrderByDescending(x => x).Take(3).ToList();
        }

        private void UpdateMonthlyTopScores(int newScore)
        {
            MonthlyTopScores.Add(newScore);
            MonthlyTopScores = MonthlyTopScores.OrderByDescending(x => x).Take(3).ToList();
        }

        private void UpdateAllTimeTopScores(int newScore)
        {
            AllTimeTopScores.Add(newScore);
            AllTimeTopScores = AllTimeTopScores.OrderByDescending(x => x).Take(3).ToList();

            if (newScore > _bestScore.Value)
            {
                _bestScore.Value = newScore;
            }
        }

        private void ResetScores(List<int> scores)
        {
            scores.Clear();
            SaveScoresToJson();
        }

        private void CheckDateAndResetIfNecessary()
        {
            DateTime currentDate = DateTime.Today;
            if (_lastPlayedDate.Date != currentDate)
            {
                // Reset today's scores if date has changed
                ResetScores(TodayTopScores);
            }

            if (_lastPlayedDate.Month != currentDate.Month ||
                _lastPlayedDate.Year != currentDate.Year)
            {
                // Reset monthly scores if month has changed
                ResetScores(MonthlyTopScores);
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
                    MonthlyTopScores = MonthlyTopScores.ToArray(),
                    AllTimeTopScores = AllTimeTopScores.ToArray(),
                    LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                };

                // Save files in the application's persistent data folder
                File.WriteAllText(s_scoresFilePath, JsonUtility.ToJson(scoreContainer));
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
                if (File.Exists(s_scoresFilePath))
                {
                    string json = File.ReadAllText(s_scoresFilePath);

                    // Deserialize ScoreContainer object from JSON string
                    ScoreContainer loadedScoreContainer = JsonUtility.FromJson<ScoreContainer>(json);

                    TodayTopScores = new List<int>(loadedScoreContainer.TodayTopScores);
                    MonthlyTopScores = new List<int>(loadedScoreContainer.MonthlyTopScores);
                    AllTimeTopScores = new List<int>(loadedScoreContainer.AllTimeTopScores);
                    _bestScore.Value = AllTimeTopScores[0];
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
                        MonthlyTopScores = new int[0],
                        AllTimeTopScores = new int[0],
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
                    MonthlyTopScores = new int[0],
                    AllTimeTopScores = new int[0],
                    LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd")
                };
            }
        }
    }
}
