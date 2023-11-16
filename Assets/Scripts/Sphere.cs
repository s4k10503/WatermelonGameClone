using UnityEngine;
using SuikaGameClone;

namespace SuikaGameClone
{
    public class Sphere : MonoBehaviour
    {
        private Rigidbody2D _rb;
        public bool _isMergeFlag = false;
        public bool _isDrop = false;
        public int _sphereNo;

        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        private float _minDiameter = 0.4f;
        private float stepSize = 0.2f;
        private bool _isTouched = false;
        private GameModel.GameState _currentState;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        void Update()
        {
            _currentState = GameManager._instance.GetCurrentState();

            if (_currentState == GameModel.GameState.SphereMoving && !_isDrop)
                _rb.simulated = false;  // Disable physics for the selected sphere
            else
                _rb.simulated = true;  // Enable physics for all other spheres

            if (Input.GetMouseButton(0) && !_isDrop)
                Drop();

            if (_isDrop && _rb.velocity.sqrMagnitude < 0.01f && _isTouched)
            {
                GameManager._instance.SetGameState(GameModel.GameState.SphereDropping);
                GameManager._instance.CheckGameOver(transform.position.y);
            }
            else if (!_isDrop)
                UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_currentState == GameModel.GameState.GameOver)
                return;

            GameManager._instance.SetGameState(GameModel.GameState.SphereMoving);

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float currentDiameter = _minDiameter + stepSize * (_sphereNo + 1);
            float offset = currentDiameter / 2 + 0.01f;
            float adjustedMinX = _minX + offset;
            float adjustedMaxX = _maxX - offset;
            mousePos.x = Mathf.Clamp(mousePos.x, adjustedMinX, adjustedMaxX);
            mousePos.y = _fixedY;
            transform.position = mousePos;
        }

        private void Drop()
        {
            if (_isDrop)
                return;

            GameManager._instance.SetGameState(GameModel.GameState.SphereDropping);

            _isDrop = true;
            _rb.velocity = Vector2.zero;
            _rb.simulated = true;
            GameManager._instance._isNext = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_currentState == GameModel.GameState.GameOver)
                return;

            if (IsEligibleForMerge(collision, out Sphere colSphere))
            {
                PerformMerge(colSphere);
            }

            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Sphere"))
            {
                _isTouched = true;
            }
        }

        private bool IsEligibleForMerge(Collision2D collision, out Sphere colSphere)
        {
            colSphere = null;
            GameObject colObj = collision.gameObject;
            if (!colObj.CompareTag("Sphere")) return false;

            colSphere = colObj.GetComponent<Sphere>();
            return _sphereNo == colSphere._sphereNo &&
                !_isMergeFlag &&
                !colSphere._isMergeFlag;
        }

        private void PerformMerge(Sphere colSphere)
        {
            GameManager._instance.SetGameState(GameModel.GameState.Merging);

            _isMergeFlag = true;
            colSphere._isMergeFlag = true;

            if (_sphereNo < GameManager._instance._maxSphereNo - 1)
                GameManager._instance.MergeNext(transform.position, _sphereNo);

            Destroy(gameObject);
            Destroy(colSphere.gameObject);
        }
    }
}
