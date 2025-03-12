using Presentation.DTO;
using Presentation.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.View.Common
{
    public sealed class DetailedScoreRankPageView : MonoBehaviour, IDetailedScoreRankPageView
    {
        [SerializeField] private GameObject[] _textsScoreRankDaily = new GameObject[7];
        [SerializeField] private GameObject[] _textsScoreRankMonthly = new GameObject[7];
        [SerializeField] private GameObject[] _textsScoreRankAllTime = new GameObject[7];
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _buttonBack;

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

            SetupButtonAnimations(_buttonBack);
        }

        private void OnDestroy()
        {
            _textsScoreRankDaily = null;
            _textsScoreRankMonthly = null;
            _textsScoreRankAllTime = null;

            _canvas = null;
            _buttonBack = null;

            _dailyScores = null;
            _monthlyScores = null;
            _allTimeScores = null;

            _uiHelper = null;
            _uiAnimator = null;
        }

        public void DisplayTopScores(ScoreContainerDto scoreContainer)
        {
            _dailyScores = scoreContainer.data.rankings.daily.scores.Take(7).ToList();
            _monthlyScores = scoreContainer.data.rankings.monthly.scores.Take(7).ToList();
            _allTimeScores = scoreContainer.data.rankings.allTime.scores.Take(7).ToList();

            UpdatePanelDisplay();
        }

        private void UpdatePanelDisplay()
        {
            _uiHelper.UpdateScoreRankPanelTexts(_textsScoreRankDaily, _dailyScores);
            _uiHelper.UpdateScoreRankPanelTexts(_textsScoreRankMonthly, _monthlyScores);
            _uiHelper.UpdateScoreRankPanelTexts(_textsScoreRankAllTime, _allTimeScores);
        }

        public void ShowPage()
        {
            _canvas.enabled = true;
        }
        public void HidePage() => _canvas.enabled = false;

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
