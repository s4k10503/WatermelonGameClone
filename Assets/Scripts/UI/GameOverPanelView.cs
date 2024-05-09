using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using Zenject;

namespace WatermelonGameClone
{
    public class GameOverPanelView : MonoBehaviour, IGameOverPanelView
    {
        // Objects
        private Transform _canvasTransform;
        private GameObject _gameOverPopupPanel;
        private GameObject _gameOverPopupInstance;

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

            DOTween.Init();
        }

        private void OnDestroy()
        {

        }

        public void ShowGameOverPopup(int score)
        {
            _gameOverPopupInstance = Instantiate(this.gameObject, _canvasTransform);
            _uiAnimator.AnimateScale(_gameOverPopupInstance, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack);

            var buttons = _gameOverPopupInstance.GetComponentsInChildren<Button>(true);
            var buttonActions = new Dictionary<string, Action>
            {
                { "Restart", () => _onRestart.OnNext(Unit.Default) },
                { "Back to title", () => _onBackToTitle.OnNext(Unit.Default) }
            };

            foreach (var button in buttons)
            {
                if (buttonActions.TryGetValue(button.name, out var action))
                {
                    button.OnClickAsObservable()
                        .Subscribe(_ => action())
                        .AddTo(this);
                }
                else
                {
                    Debug.LogWarning($"No action defined for button {button.name} in the game over popup!");
                }
            }
        }
    }
}
