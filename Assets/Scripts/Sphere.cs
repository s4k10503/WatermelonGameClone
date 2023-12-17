using UnityEngine;
using UniRx;


namespace WatermelonGameClone
{
    public class Sphere : MonoBehaviour
    {
        [Header("Sphere Information")]
        public bool IsMerge = false;
        public bool IsDrop = false;
        public int SphereNo;

        [Header("Sphere movement restrictions")]
        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        private Rigidbody2D _rb;
        private float _minDiameter = 0.4f;
        private float _stepSize = 0.2f;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        void Update()
        {
            if (IsDrop)
            {
                _rb.simulated = true;
            }
            else
            {
                UpdatePosition();
                if (Input.GetMouseButton(0))
                {
                    Drop();
                }
            }
        }

        private void UpdatePosition()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float currentDiameter = _minDiameter + _stepSize * (SphereNo + 1);
            float offset = currentDiameter / 2 + 0.01f;
            float adjustedMinX = _minX + offset;
            float adjustedMaxX = _maxX - offset;
            mousePos.x = Mathf.Clamp(mousePos.x, adjustedMinX, adjustedMaxX);
            mousePos.y = _fixedY;
            transform.position = mousePos;
        }

        private void Drop()
        {
            GameManager.Instance.GameEvent.Execute(GameModel.GameState.SphereDropping);
            IsDrop = true;
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
            return SphereNo == colSphere.SphereNo &&
                !IsMerge &&
                !colSphere.IsMerge;
        }

        private void PerformMerge(Sphere colSphere)
        {
            GameManager.Instance.GameEvent.Execute(GameModel.GameState.Merging);

            IsMerge = true;
            colSphere.IsMerge = true;

            if (SphereNo < GameManager.Instance.MaxSphereNo - 1)
            {
                GameManager.Instance.MergeNext(transform.position, SphereNo);
            }

            GameManager.Instance.GameEvent.Execute(GameModel.GameState.SphereMoving);

            Destroy(gameObject);
            Destroy(colSphere.gameObject);
        }
    }
}
