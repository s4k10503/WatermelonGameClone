using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;
using Zenject;

namespace WatermelonGameClone
{
    public class GameOverPanelView : MonoBehaviour, IGameOverPanelView
    {
        // Button
        [SerializeField] Button _buttonBackToTitle;
        [SerializeField] Button _buttonRestart;
        [SerializeField] Button _buttonScore;
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] RawImage _screenshot;

        // RankPanels
        [SerializeField] GameObject _textPanelTitle;
        [SerializeField] GameObject _textScoreRank1;
        [SerializeField] GameObject _textScoreRank2;
        [SerializeField] GameObject _textScoreRank3;
        [SerializeField] ScoreRankTextConfig _textConfig;

        // GameObjects
        private Transform _canvasTransform;
        private IInputEventProvider _inputEventProvider;
        private GameObject[] _textsScoreRanks;

        // 0 = Daily, 1 = Monthly, 2 = AllTime
        private int _currentPanelIndex = 0;
        private List<int> _dailyScores;
        private List<int> _monthlyScores;
        private List<int> _allTimeScores;

        // Subjects
        private Subject<Unit> _onRestart = new Subject<Unit>();
        private Subject<Unit> _onBackToTitle = new Subject<Unit>();

        // Observables
        public IObservable<Unit> OnRestart => _onRestart;
        public IObservable<Unit> OnBackToTitle => _onBackToTitle;

        // UIAnimator
        private IUIAnimator _uiAnimator;

        // UIHelper
        private IUIHelper _uiHelper;

        [Inject]
        public void Construct(
            IUIAnimator uiAnimator,
            IUIHelper uiHelper,
            IInputEventProvider inputEventProvider,
            [Inject(Id = "CanvasTransform")] Transform canvasTransform)
        {
            _uiAnimator = uiAnimator;
            _canvasTransform = canvasTransform;

            _inputEventProvider = inputEventProvider;
            _textsScoreRanks = new[] { _textScoreRank1, _textScoreRank2, _textScoreRank3 };

            _uiHelper = uiHelper;

            _onRestart.AddTo(this);
            _onBackToTitle.AddTo(this);

            gameObject.SetActive(false);
        }

        void Start()
        {
            _buttonBackToTitle
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _onBackToTitle.OnNext(Unit.Default);
                })
                .AddTo(this);

            _buttonRestart
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _onRestart.OnNext(Unit.Default);
                })
                .AddTo(this);

            _inputEventProvider
                .OnLeftKey
                .Subscribe(_ => ChangePanelDisplay(-1))
                .AddTo(this);

            _inputEventProvider
                .OnRightKey
                .Subscribe(_ => ChangePanelDisplay(1))
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _onRestart.Dispose();
            _onBackToTitle.Dispose();
        }

        public void ShowPanel(int score, RenderTexture screenShot, ScoreContainer scoreContainer)
        {
            _dailyScores = scoreContainer.TodayTopScores.ToList();
            _monthlyScores = scoreContainer.MonthlyTopScores.ToList();
            _allTimeScores = scoreContainer.AllTimeTopScores.ToList();

            _uiHelper.UpdateCurrentScoreText(_scoreText, score);
            UpdatePanelElements();
            _screenshot.texture = screenShot;

            gameObject.SetActive(true);
            _uiAnimator.AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
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
    }
}
