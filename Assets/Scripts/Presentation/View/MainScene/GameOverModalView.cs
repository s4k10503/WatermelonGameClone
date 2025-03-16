using Presentation.Interfaces;
using Presentation.SODefinitions;
using Presentation.DTO;

using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Zenject;

namespace Presentation.View.MainScene
{
    public sealed class GameOverModalView : MonoBehaviour, IGameOverModalView
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonBackToTitle;
        [SerializeField] private Button _buttonScore;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private RawImage _screenshot;

        [SerializeField] private GameObject _textPanelTitle;
        [SerializeField] private GameObject _textScoreRank1;
        [SerializeField] private GameObject _textScoreRank2;
        [SerializeField] private GameObject _textScoreRank3;


        private IInputEventProvider _inputEventProvider;
        private GameObject[] _textsScoreRanks;
        private ScoreRankTextConfig _textConfig;

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
            => _buttonScore.OnClickAsObservable();

        private IUIAnimator _uiAnimator;
        private Vector3 _originalScale;
        private Vector3 _pressedScale;

        private IUIHelper _uiHelper;

        [Inject]
        public void Construct(
            IUIAnimator uiAnimator,
            IUIHelper uiHelper,
            IInputEventProvider inputEventProvider,
            ScoreRankTextConfig textConfig)
        {
            _originalScale = _buttonBackToTitle.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
            _uiAnimator = uiAnimator;
            _uiHelper = uiHelper;
            _inputEventProvider = inputEventProvider;
            _textsScoreRanks = new[] { _textScoreRank1, _textScoreRank2, _textScoreRank3 };
            _textConfig = textConfig;

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
            SetupButtonAnimations(_buttonScore);
        }

        private void OnDestroy()
        {
            _dailyScores = null;
            _monthlyScores = null;
            _allTimeScores = null;
            _screenshot.texture = null;
            _textsScoreRanks = null;

            _buttonBackToTitle = null;
            _buttonRestart = null;
            _buttonScore = null;
            _scoreText = null;
            _screenshot = null;

            _textPanelTitle = null;
            _textScoreRank1 = null;
            _textScoreRank2 = null;
            _textScoreRank3 = null;
            _textConfig = null;
            _canvas = null;
        }

        public void ShowModal(int score, RenderTexture screenShot, ScoreContainerDto scoreContainer)
        {
            _canvas.enabled = true;

            _dailyScores = scoreContainer.data.rankings.daily.scores.Take(3).ToList();
            _monthlyScores = scoreContainer.data.rankings.monthly.scores.Take(3).ToList();
            _allTimeScores = scoreContainer.data.rankings.allTime.scores.Take(3).ToList();

            _uiHelper.UpdateCurrentScoreText(_scoreText, score);
            UpdatePanelElements();
            _screenshot.texture = screenShot;
            _uiAnimator.AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }

        public void ShowModalWithoutData()
        {
            _canvas.enabled = true;
            _uiAnimator.AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }

        public void HideModal()
        {
            _canvas.enabled = false;
        }

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
