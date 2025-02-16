using UnityEngine;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public sealed class ScorePanelView : MonoBehaviour, IScorePanelView
    {
        [SerializeField] private TextMeshProUGUI _currentScoreText;
        [SerializeField] private TextMeshProUGUI _bestScoreText;
        private IUIAnimator _uiAnimator;


        [Inject]
        public void Construct(IUIAnimator uiAnimator)
        {
            _uiAnimator = uiAnimator;
            _uiAnimator.GetUIPosition(transform);

            this.UpdateAsObservable()
                .Subscribe(_ => _uiAnimator.HarmonicMotion(transform, HarmonicMotionType.Sin))
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _currentScoreText = null;
            _bestScoreText = null;

            _uiAnimator = null;
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
