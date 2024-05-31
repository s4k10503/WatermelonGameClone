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
        [SerializeField] GameObject _textPanelTitle;
        [SerializeField] GameObject _textScoreRank1;
        [SerializeField] GameObject _textScoreRank2;
        [SerializeField] GameObject _textScoreRank3;
        [SerializeField] GameObject _textCurrentScore;
        [SerializeField] ScoreRankTextConfig _textConfig;

        // GameObject references for input
        private IInputEventProvider _inputEventProvider;
        private GameObject[] _textsScoreRanks;

        // 0 = Daily, 1 = Monthly, 2 = AllTime
        private int _currentPanelIndex = 0;

        private List<int> _dailyScores;
        private List<int> _monthlyScores;
        private List<int> _allTimeScores;

        [Inject]
        public void Construct(IInputEventProvider inputEventProvider)
        {
            _inputEventProvider = inputEventProvider;
            _textsScoreRanks = new[] { _textScoreRank1, _textScoreRank2, _textScoreRank3 };
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
            _dailyScores = dailyScores;
            _monthlyScores = monthlyScores;
            _allTimeScores = allTimeScores;

            UpdatePanelDisplay();
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

        private void SetTitleText(GameObject textPanelTitle, string titleText)
        {
            textPanelTitle
                .GetComponent<TextMeshProUGUI>()
                .SetText(titleText);
        }

        private void ChangePanelDisplay(int direction)
        {
            _currentPanelIndex = (_currentPanelIndex + direction + 3) % 3;
            UpdatePanelDisplay();
        }

        private void UpdatePanelDisplay()
        {
            List<int> scoresToDisplay = null;
            string titleText = "";

            switch (_currentPanelIndex)
            {
                case 0:
                    scoresToDisplay = _dailyScores;
                    titleText = _textConfig.dailyRankingTitle;
                    break;
                case 1:
                    scoresToDisplay = _monthlyScores;
                    titleText = _textConfig.monthlyRankingTitle;
                    break;
                case 2:
                    scoresToDisplay = _allTimeScores;
                    titleText = _textConfig.allTimeRankingTitle;
                    break;
            }

            SetTitleText(_textPanelTitle, titleText);
            SetScoreTexts(_textsScoreRanks, scoresToDisplay);
        }
    }
}
