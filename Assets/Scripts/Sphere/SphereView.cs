using UnityEngine;
using System;
using UniRx;
using Zenject;


namespace WatermelonGameClone
{
    public class SphereView : MonoBehaviour, ISphereView
    {
        [Header("Movement restrictions")]
        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        private IInputEventProvider _inputEventProvider;

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

        // Subjects
        private Subject<Unit> _onGameOver = new Subject<Unit>();
        private Subject<Unit> _onDropping = new Subject<Unit>();
        private Subject<Unit> _onMoving = new Subject<Unit>();
        private Subject<MergeData> _onMerging = new Subject<MergeData>();

        // Observables
        public IObservable<Unit> OnGameOver => _onGameOver;
        public IObservable<Unit> OnDropping => _onDropping;
        public IObservable<Unit> OnMoving => _onMoving;
        public IObservable<MergeData> OnMerging => _onMerging;

        // ReactiveProperty
        private ReactiveProperty<int> _nextSphereIndex = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> NextSphereIndex => _nextSphereIndex.ToReadOnlyReactiveProperty();


        [Inject]
        public void Construct(IInputEventProvider inputEventProvider)
        {
            _inputEventProvider = inputEventProvider;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        private void Start()
        {
            _onGameOver.AddTo(this);
            _onDropping.AddTo(this);
            _onMoving.AddTo(this);
            _onMerging.AddTo(this);

            _inputEventProvider
                .OnMouseMove
                .Where(_ => !_isDrop)
                .Subscribe(UpdatePosition)
                .AddTo(this);

            _inputEventProvider
                .OnMouseClick
                .Where(_ => !_isDrop)
                .Subscribe(_ => StartDropping())
                .AddTo(this);
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
                    _onGameOver.OnNext(Unit.Default);
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

        private void UpdatePosition(Vector2 mousePos)
        {
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

            _onDropping.OnNext(Unit.Default);
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

                _onMerging.OnNext(new MergeData(newPosition, _sphereNo, gameObject, otherSphere.gameObject));
            }
            _onMoving.OnNext(Unit.Default);
        }
    }
}