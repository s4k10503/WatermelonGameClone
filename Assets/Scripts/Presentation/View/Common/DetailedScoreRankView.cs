using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public sealed class DetailedScoreRankView : MonoBehaviour, IDetailedScoreRankView
    {
        [SerializeField] GameObject[] _textsScoreRankDaily = new GameObject[7];
        [SerializeField] GameObject[] _textsScoreRankMonthly = new GameObject[7];
        [SerializeField] GameObject[] _textsScoreRankAllTime = new GameObject[7];
        [SerializeField] Canvas _canvas;

        [SerializeField] Button _buttonBack;

        private IUIHelper _uiHelper;
        private IUIAnimator _uiAnimator;
        private Vector3 _originalScale;
        private Vector3 _pressedScale;

        private List<int> _dailyScores;
        private List<int> _monthlyScores;
        private List<int> _allTimeScores;

        public IObservable<Unit> OnBack => _buttonBack.OnClickAsObservable();


        [Inject]
        public void Construct(IUIHelper uiHelper, IUIAnimator uiAnimator)
        {
            _uiHelper = uiHelper;
            _uiAnimator = uiAnimator;
            _originalScale = _buttonBack.transform.localScale;
            _pressedScale = _originalScale * 0.9f;
        }

        void Start()
        {
            HidePanel();
            SetupButtonAnimations(_buttonBack);
        }

        public void DisplayTopScores(ScoreContainer scoreContainer)
        {
            _dailyScores = scoreContainer.Data.Rankings.Daily.Scores.Take(7).ToList();
            _monthlyScores = scoreContainer.Data.Rankings.Monthly.Scores.Take(7).ToList();
            _allTimeScores = scoreContainer.Data.Rankings.AllTime.Scores.Take(7).ToList();

            UpdatePanelDisplay();
        }

        private void UpdatePanelDisplay()
        {
            _uiHelper.UpdateScoreRankPanelTexts(_textsScoreRankDaily, _dailyScores);
            _uiHelper.UpdateScoreRankPanelTexts(_textsScoreRankMonthly, _monthlyScores);
            _uiHelper.UpdateScoreRankPanelTexts(_textsScoreRankAllTime, _allTimeScores);
        }

        public void ShowPanel()
        {
            _canvas.enabled = true;
        }
        public void HidePanel() => _canvas.enabled = false;

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
