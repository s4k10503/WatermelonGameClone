using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace WatermelonGameClone
{
    public class GameView : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] GameObject _scorePanel;
        [SerializeField] GameObject _nextSpherePanel;
        [SerializeField] GameObject _rankingPanel;
        [SerializeField] GameObject _evolutionCirclePanel;
        [SerializeField] Transform _canvasTransform;
        [SerializeField] GameObject _gameOverPopupPrefab;

        [Header("Parameters")]
        [SerializeField, Range(0f, 10f)] float _moveSpeed = 0f;
        [SerializeField, Range(0f, 10f)] float _moveHeight = 0f;

        private Subject<Unit> _onRestartRequested = new Subject<Unit>();
        public IObservable<Unit> OnRestartRequested => _onRestartRequested;

        private Subject<Unit> _onBackToTitleRequested = new Subject<Unit>();
        public IObservable<Unit> OnBackToTitleRequested => _onBackToTitleRequested;

        private Vector3 _scorePanelPos;
        private Vector3 _nextSpherePanelPos;

        private GameObject _gameOverPopupInstance;

        public void UpdateCurrentScore(int currentScore)
        {
            var currentScoreUI = _scorePanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            currentScoreUI.SetText(currentScore.ToString());
        }

        public void UpdateBestScore(int bestScore)
        {
            var bestScoreUI = _scorePanel.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            bestScoreUI.SetText(bestScore.ToString());
        }

        private void MoveUI()
        {
            _scorePanel.transform.localPosition = _scorePanelPos + new Vector3(0f, Mathf.Sin(Time.time * _moveSpeed) * _moveHeight, 0f);
            _nextSpherePanel.transform.localPosition = _nextSpherePanelPos + new Vector3(0f, Mathf.Cos(Time.time * _moveSpeed) * _moveHeight, 0f);
        }

        private void GetUIPos()
        {
            _scorePanelPos = _scorePanel.transform.localPosition;
            _nextSpherePanelPos = _nextSpherePanel.transform.localPosition;
        }

        public void ShowGameOverPopup()
        {
            _gameOverPopupInstance = Instantiate(_gameOverPopupPrefab, _canvasTransform);
            var buttons = _gameOverPopupInstance.GetComponentsInChildren<Button>(true);
            var buttonActions = new Dictionary<string, Action>
            {
                { "Restart", () => _onRestartRequested.OnNext(Unit.Default) },
                { "Back to title", () => _onBackToTitleRequested.OnNext(Unit.Default) }
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

        private void Start()
        {
            GetUIPos();
        }

        private void Update()
        {
            MoveUI();
        }
    }
}
