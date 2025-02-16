using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public sealed class ScoreRankPanelView : MonoBehaviour, IScoreRankView
    {
        [SerializeField] private GameObject _textPanelTitle;
        [SerializeField] private GameObject _textScoreRank1;
        [SerializeField] private GameObject _textScoreRank2;
        [SerializeField] private GameObject _textScoreRank3;
        [SerializeField] private GameObject _textCurrentScore;
        [SerializeField] private ScoreRankTextConfig _textConfig;

        private IInputEventProvider _inputEventProvider;
        private GameObject[] _textsScoreRanks;

        private IUIHelper _uiHelper;

        // 0 = Daily, 1 = Monthly, 2 = AllTime
        private int _currentPanelIndex = 0;
        private List<int> _dailyScores;
        private List<int> _monthlyScores;
        private List<int> _allTimeScores;

        [Inject]
        public void Construct(IInputEventProvider inputEventProvider, IUIHelper uiHelper)
        {
            _textsScoreRanks = new[] { _textScoreRank1, _textScoreRank2, _textScoreRank3 };
            _uiHelper = uiHelper;
            _inputEventProvider = inputEventProvider;

            _inputEventProvider
                .OnLeftKey
                .Where(_ => Time.timeScale != 0f)
                .Subscribe(_ => ChangePanelDisplay(-1))
                .AddTo(this);

            _inputEventProvider
                .OnRightKey
                .Where(_ => Time.timeScale != 0f)
                .Subscribe(_ => ChangePanelDisplay(1))
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _textPanelTitle = null;
            _textScoreRank1 = null;
            _textScoreRank2 = null;
            _textScoreRank3 = null;
            _textCurrentScore = null;
            _textConfig = null;

            _inputEventProvider = null;
            _textsScoreRanks = null;
            _dailyScores = null;
            _monthlyScores = null;
            _allTimeScores = null;
        }

        public void DisplayCurrentScore(int currentScore)
        {
            _uiHelper
                .UpdateCurrentScoreText(_textCurrentScore.GetComponent<TextMeshProUGUI>(), currentScore);
        }

        public void DisplayTopScores(ScoreContainer scoreContainer)
        {
            _dailyScores = scoreContainer.Data.Rankings.Daily.Scores.Take(3).ToList();
            _monthlyScores = scoreContainer.Data.Rankings.Monthly.Scores.Take(3).ToList();
            _allTimeScores = scoreContainer.Data.Rankings.AllTime.Scores.Take(3).ToList();
            UpdatePanelDisplay();
        }

        private void ChangePanelDisplay(int direction)
        {
            _currentPanelIndex = (_currentPanelIndex + direction + 3) % 3;
            UpdatePanelDisplay();
        }

        private void UpdatePanelDisplay()
        {
            var (scores, title) = GetScoresAndTitleByIndex(_currentPanelIndex);

            _uiHelper.UpdateTitlePanelText(_textPanelTitle, title);
            _uiHelper.UpdateScoreRankPanelTexts(_textsScoreRanks, scores);
        }

        private (List<int> scores, string title) GetScoresAndTitleByIndex(int index)
        {
            return index switch
            {
                0 => (_dailyScores, _textConfig.dailyRankingTitle),
                1 => (_monthlyScores, _textConfig.monthlyRankingTitle),
                2 => (_allTimeScores, _textConfig.allTimeRankingTitle),
                _ => (new List<int>(), "Invalid Index"),
            };
        }
    }
}
