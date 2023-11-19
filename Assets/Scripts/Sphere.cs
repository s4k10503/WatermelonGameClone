using UnityEngine;
using SuikaGameClone;

namespace SuikaGameClone
{
    public class Sphere : MonoBehaviour
    {
        private Rigidbody2D _rb;
        public bool _isMerge = false;
        public bool _isDrop = false;
        public int _sphereNo;

        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        private float _minDiameter = 0.4f;
        private float _stepSize = 0.2f;
        private GameModel.GameState _currentState;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        void Update()
        {
            _currentState = GameManager.Instance.GetCurrentState();

            if (_isDrop)
            {
                _rb.simulated = true;
            }
            else
            {
                if (Input.GetMouseButton(0))
                    Drop();

                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float currentDiameter = _minDiameter + _stepSize * (_sphereNo + 1);
            float offset = currentDiameter / 2 + 0.01f;
            float adjustedMinX = _minX + offset;
            float adjustedMaxX = _maxX - offset;
            mousePos.x = Mathf.Clamp(mousePos.x, adjustedMinX, adjustedMaxX);
            mousePos.y = _fixedY;
            transform.position = mousePos;
        }

        private void Drop()
        {
            GameManager.Instance.SetGameState(GameModel.GameState.SphereDropping);

            _isDrop = true;
            _rb.velocity = Vector2.zero;
            _rb.simulated = true;
            GameManager.Instance.IsNext = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsEligibleForMerge(collision, out Sphere colSphere))
                PerformMerge(colSphere);
        }

        private bool IsEligibleForMerge(Collision2D collision, out Sphere colSphere)
        {
            colSphere = null;
            GameObject colObj = collision.gameObject;
            if (!colObj.CompareTag("Sphere"))
                return false;

            colSphere = colObj.GetComponent<Sphere>();
            return _sphereNo == colSphere._sphereNo &&
                !_isMerge &&
                !colSphere._isMerge;
        }

        private void PerformMerge(Sphere colSphere)
        {
            _isMerge = true;
            colSphere._isMerge = true;

            if (_sphereNo < GameManager.Instance.MaxSphereNo - 1)
                GameManager.Instance.MergeNext(transform.position, _sphereNo);

            Destroy(gameObject);
            Destroy(colSphere.gameObject);
        }
    }
}
