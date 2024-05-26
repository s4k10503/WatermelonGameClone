using System;
using System.Collections.Generic;
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

        // Objects
        private Transform _canvasTransform;

        // Subjects
        private Subject<Unit> _onRestart = new Subject<Unit>();
        private Subject<Unit> _onBackToTitle = new Subject<Unit>();

        // Observables
        public IObservable<Unit> OnRestart => _onRestart;
        public IObservable<Unit> OnBackToTitle => _onBackToTitle;

        private IUIAnimator _uiAnimator;


        [Inject]
        public void Construct(
            IUIAnimator uiAnimator,

            [Inject(Id = "CanvasTransform")] Transform canvasTransform)
        {
            _uiAnimator = uiAnimator;
            _canvasTransform = canvasTransform;

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
        }

        private void OnDestroy()
        {

        }

        public void ShowPanel(int score, RenderTexture screenShot)
        {
            UpdateScoreText(_scoreText, score);
            _screenshot.texture = screenShot;

            gameObject.SetActive(true);
            _uiAnimator.AnimateScale(gameObject, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);
        }

        private void UpdateScoreText(TextMeshProUGUI textMesh, int score)
        {
            textMesh.SetText(score.ToString());
        }
    }
}
