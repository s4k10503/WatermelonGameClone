using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class ScoreRankView : MonoBehaviour, IScoreRankView
    {
        // GameObject references for input
        private IInputEventProvider _inputEventProvider;

        // GameObject references for daily, monthly, and all-time scores
        private GameObject _panelDailyRanking;
        private GameObject _panelMonthlyRanking;
        private GameObject _panelAllTimeRanking;

        private GameObject[] _textsDailyScoreRanks;
        private GameObject[] _textsMonthlyScoreRanks;
        private GameObject[] _textsAllTimeScoreRanks;

        private GameObject _textCurrentScore;

        // 0 = Daily, 1 = Monthly, 2 = AllTime
        private int _currentPanelIndex = 0;

        [Inject]
        public void Construct(
            IInputEventProvider inputEventProvider,

            [Inject(Id = "PanelDailyRanking")] GameObject panelDailyRanking,
            [Inject(Id = "TextDailyScoreRank1")] GameObject textDailyScoreRank1,
            [Inject(Id = "TextDailyScoreRank2")] GameObject textDailyScoreRank2,
            [Inject(Id = "TextDailyScoreRank3")] GameObject textDailyScoreRank3,

            [Inject(Id = "PanelMonthlyRanking")] GameObject panelMonthlyRanking,
            [Inject(Id = "TextMonthlyScoreRank1")] GameObject textMonthlyScoreRank1,
            [Inject(Id = "TextMonthlyScoreRank2")] GameObject textMonthlyScoreRank2,
            [Inject(Id = "TextMonthlyScoreRank3")] GameObject textMonthlyScoreRank3,

            [Inject(Id = "PanelAllTimeRanking")] GameObject panelAllTimeRanking,
            [Inject(Id = "TextAllTimeScoreRank1")] GameObject textAllTimeScoreRank1,
            [Inject(Id = "TextAllTimeScoreRank2")] GameObject textAllTimeScoreRank2,
            [Inject(Id = "TextAllTimeScoreRank3")] GameObject textAllTimeScoreRank3,

            [Inject(Id = "TextCurrentScore")] GameObject textCurrentScore)
        {
            _inputEventProvider = inputEventProvider;

            _panelDailyRanking = panelDailyRanking;
            _panelMonthlyRanking = panelMonthlyRanking;
            _panelAllTimeRanking = panelAllTimeRanking;

            _textsDailyScoreRanks = new[] { textDailyScoreRank1, textDailyScoreRank2, textDailyScoreRank3 };
            _textsMonthlyScoreRanks = new[] { textMonthlyScoreRank1, textMonthlyScoreRank2, textMonthlyScoreRank3 };
            _textsAllTimeScoreRanks = new[] { textAllTimeScoreRank1, textAllTimeScoreRank2, textAllTimeScoreRank3 };
            _textCurrentScore = textCurrentScore;
        }

        private void Start()
        {
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

        public void DisplayCurrentScore(int currentScore)
        {
            _textCurrentScore
                .GetComponent<TextMeshProUGUI>()
                .SetText(currentScore.ToString());
        }

        public void DisplayTopScores(List<int> dailyScores, List<int> monthlyScores, List<int> allTimeScores)
        {
            SetScoreTexts(_textsDailyScoreRanks, dailyScores);
            SetScoreTexts(_textsMonthlyScoreRanks, monthlyScores);
            SetScoreTexts(_textsAllTimeScoreRanks, allTimeScores);
        }

        private void SetScoreTexts(GameObject[] rankTexts, List<int> scores)
        {
            for (int i = 0; i < rankTexts.Length; i++)
            {
                string scoreText = (scores.Count > i && rankTexts[i] != null) ? scores[i].ToString() : "--";
                rankTexts[i]
                    .GetComponent<TextMeshProUGUI>()
                    .SetText(scoreText);
            }
        }

        private void ChangePanelDisplay(int direction)
        {
            _currentPanelIndex = (_currentPanelIndex + direction + 3) % 3;
            _panelDailyRanking.SetActive(_currentPanelIndex == 0);
            _panelMonthlyRanking.SetActive(_currentPanelIndex == 1);
            _panelAllTimeRanking.SetActive(_currentPanelIndex == 2);
        }
    }
}
