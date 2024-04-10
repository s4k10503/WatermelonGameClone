using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

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
        [SerializeField] GameObject[] _nextSphereImage;

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
        private GameObject[] _instantiatedSpheres;

        private CompositeDisposable _disposables = new CompositeDisposable();

        public void Initialize()
        {
            DOTween.Init();
            GetUIPos();
            CreateNextSphereImages();
        }

        private void OnDestroy()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        public void CreateNextSphereImages()
        {
            _instantiatedSpheres = new GameObject[_nextSphereImage.Length];
            for (int i = 0; i < _nextSphereImage.Length; i++)
            {
                GameObject instantiatedSphere = Instantiate(_nextSphereImage[i], _nextSpherePanel.transform);
                instantiatedSphere.SetActive(false);
                _instantiatedSpheres[i] = instantiatedSphere;
                instantiatedSphere.transform.SetAsLastSibling();
            }
        }

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

        public void UpdateNextSphereImages(int nextSphereIndex)
        {
            for (int i = 0; i < _instantiatedSpheres.Length; i++)
            {
                _instantiatedSpheres[i].SetActive(i == nextSphereIndex);
            }
        }

        public void MoveUI()
        {
            _scorePanel.transform.localPosition = _scorePanelPos + new Vector3(0f, Mathf.Sin(Time.time * _moveSpeed) * _moveHeight, 0f);
            _nextSpherePanel.transform.localPosition = _nextSpherePanelPos + new Vector3(0f, Mathf.Cos(Time.time * _moveSpeed) * _moveHeight, 0f);
        }

        private void GetUIPos()
        {
            _scorePanelPos = _scorePanel.transform.localPosition;
            _nextSpherePanelPos = _nextSpherePanel.transform.localPosition;
        }

        public void ShowGameOverPopup(int score)
        {
            _gameOverPopupInstance = Instantiate(_gameOverPopupPrefab, _canvasTransform);
            _gameOverPopupInstance.transform.localScale = Vector3.zero;
            _gameOverPopupInstance.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

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
                        .AddTo(_disposables);
                }
                else
                {
                    Debug.LogWarning($"No action defined for button {button.name} in the game over popup!");
                }
            }
        }
    }
}
