using System;
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

            var restartButton = _gameOverPopupInstance.GetComponentInChildren<Button>(true);
            restartButton.OnClickAsObservable()
                .Subscribe(_ => _onRestartRequested.OnNext(Unit.Default))
                .AddTo(this);
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
