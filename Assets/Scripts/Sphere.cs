using UnityEngine;
using System;
using UniRx;


namespace WatermelonGameClone
{
    public class Sphere : MonoBehaviour
    {
        [Header("Movement restrictions")]
        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        // Flag
        public bool _isMerge = false;
        public bool _isDrop = false;

        // Paramerters
        private Rigidbody2D _rb;
        private float _minDiameter = 0.4f;
        private float _stepSize = 0.2f;
        private float _ceilingContactTime;
        private static readonly float s_timeLimit = 0.5f;
        private int _maxSphereNo;
        private int _sphereNo;

        // ReactiveProperty
        private Subject<Unit> _onGameOverRequest = new Subject<Unit>();
        public IObservable<Unit> OnGameOverRequest => _onGameOverRequest;

        private Subject<Unit> _onDroppingRequest = new Subject<Unit>();
        public IObservable<Unit> OnDroppingRequest => _onDroppingRequest;

        private Subject<Unit> _onMovingRequest = new Subject<Unit>();
        public IObservable<Unit> OnMovingRequest => _onMovingRequest;

        private Subject<MergeData> _onMergingRequest = new Subject<MergeData>();
        public IObservable<MergeData> OnMergingRequest => _onMergingRequest;

        public class MergeData
        {
            public Vector3 Position;
            public int SphereNo;
            public GameObject SphereA;
            public GameObject SphereB;

            public MergeData(Vector3 position, int sphereNo, GameObject sphereA, GameObject sphereB)
            {
                Position = position;
                SphereNo = sphereNo;
                SphereA = sphereA;
                SphereB = sphereB;
            }
        }

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        void Update()
        {
            if (_isDrop)
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsEligibleForMerge(collision, out Sphere colSphere))
                PerformMerge(colSphere);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Ceiling"))
            {
                _ceilingContactTime += Time.deltaTime;
                if (_ceilingContactTime > s_timeLimit)
                {
                    _onGameOverRequest.OnNext(Unit.Default);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ceiling"))
                _ceilingContactTime = 0;
        }

        public void Initialize(int maxSphereNo, int sphereNo)
        {
            _maxSphereNo = maxSphereNo;
            _sphereNo = sphereNo;
        }

        public void InitializeAfterMerge(int maxSphereNo, int sphereNo)
        {
            _maxSphereNo = maxSphereNo;
            _sphereNo = sphereNo;
            _isDrop = true;
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
            _onDroppingRequest.OnNext(Unit.Default);

            _isDrop = true;
            _rb.velocity = Vector2.zero;
            _rb.simulated = true;
        }

        private bool IsEligibleForMerge(Collision2D collision, out Sphere colSphere)
        {
            colSphere = null;
            GameObject colObj = collision.gameObject;
            if (!colObj.CompareTag("Sphere"))
                return false;

            colSphere = colObj.GetComponent<Sphere>();
            return _sphereNo == colSphere._sphereNo;
        }

        private void PerformMerge(Sphere colSphere)
        {
            if (gameObject.GetInstanceID() < colSphere.gameObject.GetInstanceID())
            {
                if (_sphereNo < _maxSphereNo - 1)
                {
                    var newPosition = (transform.position + colSphere.transform.position) / 2;
                    _onMergingRequest.OnNext(new MergeData(newPosition, _sphereNo, gameObject, colSphere.gameObject));
                    GamePresenter.Instance.MergeSphere(newPosition, _sphereNo);
                }
            }
            _onMovingRequest.OnNext(Unit.Default);

            Destroy(gameObject);
            Destroy(colSphere.gameObject);
        }
    }
}
