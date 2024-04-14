using UnityEngine;
using System;
using UniRx;


namespace WatermelonGameClone
{
    public class SphereView : MonoBehaviour
    {
        [Header("Movement restrictions")]
        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        // Flag
        public bool _isMerge = false;
        public bool _isDrop = false;

        // Physics
        private Rigidbody2D _rb;
        private float _minDiameter = 0.4f;
        private float _stepSize = 0.2f;
        private float _ceilingContactTime;
        private static readonly float s_timeLimit = 0.5f;

        // Game Logic
        private int _maxSphereNo;
        private int _sphereNo;

        // Observables
        private Subject<Unit> _onGameOverRequest = new Subject<Unit>();
        public IObservable<Unit> OnGameOverRequest => _onGameOverRequest;

        private Subject<Unit> _onDroppingRequest = new Subject<Unit>();
        public IObservable<Unit> OnDroppingRequest => _onDroppingRequest;

        private Subject<Unit> _onMovingRequest = new Subject<Unit>();
        public IObservable<Unit> OnMovingRequest => _onMovingRequest;

        private Subject<MergeData> _onMergingRequest = new Subject<MergeData>();
        public IObservable<MergeData> OnMergingRequest => _onMergingRequest;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        void Update()
        {
            HandleUserInput();
        }

        private void HandleUserInput()
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
                    StartDropping();
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsEligibleForMerge(collision, out SphereView otherSphere))
                PerformMerge(otherSphere);
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

        private void StartDropping()
        {
            _isDrop = true;
            _rb.velocity = Vector2.zero;
            _rb.simulated = true;

            _onDroppingRequest.OnNext(Unit.Default);
        }

        private bool IsEligibleForMerge(Collision2D collision, out SphereView otherSphere)
        {
            otherSphere = null;
            GameObject colObj = collision.gameObject;
            if (!colObj.CompareTag("Sphere"))
                return false;

            otherSphere = colObj.GetComponent<SphereView>();
            return _sphereNo == otherSphere._sphereNo;
        }

        private void PerformMerge(SphereView otherSphere)
        {
            if (gameObject.GetInstanceID() < otherSphere.gameObject.GetInstanceID() && _sphereNo < _maxSphereNo - 1)
            {
                var newPosition = (transform.position + otherSphere.transform.position) / 2;
                Destroy(gameObject);
                Destroy(otherSphere.gameObject);

                _onMergingRequest.OnNext(new MergeData(newPosition, _sphereNo, gameObject, otherSphere.gameObject));
            }
            _onMovingRequest.OnNext(Unit.Default);
        }
    }
}