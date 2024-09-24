using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public sealed class GameOverPanelView : MonoBehaviour, IGameOverPanelView
    {
        [SerializeField] Button _buttonBackToTitle;
        [SerializeField] Button _buttonRestart;
        [SerializeField] Button _buttonDisplayScore;
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] RawImage _screenshot;

        [SerializeField] GameObject _textPanelTitle;
        [SerializeField] GameObject _textScoreRank1;
        [SerializeField] GameObject _textScoreRank2;
        [SerializeField] GameObject _textScoreRank3;
        [SerializeField] ScoreRankTextConfig _textConfig;

        [SerializeField] Canvas _canvas;
        private IInputEventProvider _inputEventProvider;
        private GameObject[] _textsScoreRanks;

        // 0 = Daily, 1 = Monthly, 2 = AllTime
        private int _currentPanelIndex = 0;
        private List<int> _dailyScores;
        private List<int> _monthlyScores;
        private List<int> _allTimeScores;

        public IObservable<Unit> OnRestart
            => _buttonRestart.OnClickAsObservable();
        public IObservable<Unit> OnBackToTitle
            => _buttonBackToTitle.OnClickAsObservable();
        public IObservable<Unit> OnDisplayScore
            => _buttonDisplayScore.OnClickAsObservable();

        private IUIAnimator _uiAnimator;
        private Vector3 _originalScale;
        private Vector3 _pressedScale;

        private IUIHelper _uiHelper;

        [Inject]
        public void Construct(
            IUIAnimator uiAnimator,
            IUIHelper uiHelper,
            IInputEventProvider inputEventProvider)
        {
            _originalScale = _buttonBackToTitle.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
            _uiAnimator = uiAnimator;
            _uiHelper = uiHelper;
            _inputEventProvider = inputEventProvider;
            _textsScoreRanks = new[] { _textScoreRank1, _textScoreRank2, _textScoreRank3 };
        }

        private void Start()
        {
            HidePanel();

            _inputEventProvider
                .OnLeftKey
                .Where(_ => Time.timeScale == 0f)
                .Subscribe(_ => ChangePanelDisplay(-1))
                .AddTo(this);

            _inputEventProvider
                .OnRightKey
                .Where(_ => Time.timeScale == 0f)
                .Subscribe(_ => ChangePanelDisplay(1))
                .AddTo(this);

            SetupButtonAnimations(_buttonBackToTitle);
            SetupButtonAnimations(_buttonRestart);
            SetupButtonAnimations(_buttonDisplayScore);
        }

        public void ShowPanel(int score, RenderTexture screenShot, ScoreContainer scoreContainer)
        {
            _canvas.enabled = true;

            _dailyScores = scoreContainer.Data.Rankings.Daily.Scores.Take(3).ToList();
            _monthlyScores = scoreContainer.Data.Rankings.Monthly.Scores.Take(3).ToList();
            _allTimeScores = scoreContainer.Data.Rankings.AllTime.Scores.Take(3).ToList();

            _uiHelper.UpdateCurrentScoreText(_scoreText, score);
            UpdatePanelElements();
            _screenshot.texture = screenShot;
            _uiAnimator.AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }

        public void ShowPanelWihtoutData()
        {
            _canvas.enabled = true;
            _uiAnimator.AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }

        public void HidePanel()
            => _canvas.enabled = false;

        private void ChangePanelDisplay(int direction)
        {
            _currentPanelIndex = (_currentPanelIndex + direction + 3) % 3;
            UpdatePanelElements();
        }

        private void UpdatePanelElements()
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

        private void SetupButtonAnimations(Button button)
        {
            // Processing when the button is pressed
            button.OnPointerDownAsObservable()
                .Subscribe(_ =>
                {
                    _uiAnimator
                        .AnimateScale(
                            button.gameObject, _originalScale, _pressedScale, 0.1f, Ease.OutQuad);
                })
                .AddTo(this);

            // Processing when the button is released
            button.OnPointerUpAsObservable()
                .Subscribe(_ =>
                {
                    _uiAnimator
                        .AnimateScale(
                            button.gameObject, _pressedScale, _originalScale, 0.1f, Ease.OutQuad);
                })
                .AddTo(this);
        }
    }
}
