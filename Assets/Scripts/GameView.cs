using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SuikaGameClone;

namespace SuikaGameClone
{
    public class GameView : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] GameObject _scorePanel;
        [SerializeField] GameObject _nextSpherePanel;
        [SerializeField] GameObject _rankingPanel;
        [SerializeField] GameObject _evolutionCirclePanel;


        [Header("Parameter")]
        [SerializeField, Range(0f, 10f)] float _moveSpeed = 0f;
        [SerializeField, Range(0f, 10f)] float _moveHeight = 0f;

        private Vector3 _scorePanelPos;
        private Vector3 _nextSpherePanelPos;


        public void UpdateScore(int newScore)
        {
            var scoreUI = _scorePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            scoreUI.SetText(newScore.ToString());
            // Debug.Log("Score: " + newScore.ToString());
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
