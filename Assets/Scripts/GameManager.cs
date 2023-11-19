using UnityEngine;
using SuikaGameClone;

namespace SuikaGameClone
{
    public class GameManager : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private Sphere[] _spherePrefab;
        [SerializeField] private Transform _spherePosition;
        [SerializeField] private GameView _gameView;


        private GameModel _gameModel = new GameModel();
        private readonly float _invokeTime = 1.0f;

        public static GameManager _instance { get; private set; }
        public bool _isNext { get; set; }
        public int _maxSphereNo { get; private set; }

        public float _ceilingYPosition = 2.5f;


        void Start()
        {
            _gameModel.ChangeState(GameModel.GameState.Initializing);

            _instance = this;
            _isNext = false;
            _maxSphereNo = _spherePrefab.Length;

            _gameView = GameObject.Find("GameView").GetComponent<GameView>();

            SetBestScore();
            CreateSphere();
        }

        void Update()
        {
            if (_isNext)
            {
                _isNext = false;
                Invoke("CreateSphere", _invokeTime);
            }
        }

        public void SetCurrentScore(int _sphereNo)
        {
            _gameModel.CalcScore(_sphereNo);
            _gameView.UpdateCurrentScore(_gameModel.CurrentScore.Value);
        }

        public void SetBestScore()
        {
            int pastBestScore = PlayerPrefs.GetInt("BestScore");
            if (_gameModel.CurrentScore.Value > pastBestScore)
            {
                _gameModel.SaveBestScore(_gameModel.CurrentScore.Value);
                _gameView.UpdateBestScore(_gameModel.BestScore.Value);
            }
        }

        private void CreateSphere()
        {
            SetGameState(GameModel.GameState.SphereMoving);

            int maxIndex = _maxSphereNo / 2 - 1;
            int i = Random.Range(0, maxIndex + 1);

            Sphere sphereIns = Instantiate(_spherePrefab[i], _spherePosition);
            sphereIns._sphereNo = i;
            sphereIns.gameObject.SetActive(true);
        }

        public void MergeNext(Vector3 target, int _sphereNo)
        {
            SetGameState(GameModel.GameState.Merging);

            Sphere sphereIns = Instantiate(_spherePrefab[_sphereNo + 1], target, Quaternion.identity, _spherePosition);
            sphereIns._sphereNo = _sphereNo + 1;
            sphereIns._isDrop = true;
            sphereIns.GetComponent<Rigidbody2D>().simulated = true;
            sphereIns.gameObject.SetActive(true);
            SetCurrentScore(_sphereNo);
        }

        public void CheckGameOver(float sphereYPosition)
        {
            if (sphereYPosition >= _ceilingYPosition)
            {
                SetGameState(GameModel.GameState.GameOver);
                SetBestScore();
                Debug.Log("sphereYPosition: " + sphereYPosition);
                Debug.Log("Game Over");
                Time.timeScale = 0f;
            }
        }

        public GameModel.GameState GetCurrentState()
        {
            return _gameModel.CurrentState.Value;
        }

        public void SetGameState(GameModel.GameState newState)
        {
            _gameModel.ChangeState(newState);
        }
    }
}