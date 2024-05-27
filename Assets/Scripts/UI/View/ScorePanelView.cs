using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class ScorePanelView : MonoBehaviour, IScorePanelView
    {
        [SerializeField] private TextMeshProUGUI _currentScoreText;
        [SerializeField] private TextMeshProUGUI _bestScoreText;
        private IUIAnimator _uiAnimator;


        [Inject]
        public void Construct(IUIAnimator uiAnimator)
        {
            _uiAnimator = uiAnimator;
            _uiAnimator.GetUIPosition(this.transform);
        }

        private void Update()
        {
            _uiAnimator.HarmonicMotion(this.transform, HarmonicMotionType.Cos);
        }

        private void OnDestroy()
        {

        }

        public void UpdateCurrentScore(int score)
        {
            UpdateScoreText(_currentScoreText, score);
        }

        public void UpdateBestScore(int score)
        {
            UpdateScoreText(_bestScoreText, score);
        }

        private void UpdateScoreText(TextMeshProUGUI textMesh, int score)
        {
            textMesh.SetText(score.ToString());
        }
    }
}
