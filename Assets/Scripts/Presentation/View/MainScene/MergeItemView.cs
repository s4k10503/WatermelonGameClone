using UnityEngine;
using System;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public sealed class MergeItemView : MonoBehaviour, IMergeItemView
    {
        [Header("Movement restrictions")]
        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        public bool _isDrop { get; private set; }
        private bool _isGameOver = false;

        // Physics
        private Rigidbody2D _rb;
        private readonly float _minDiameter = 0.4f;
        private readonly float _stepSize = 0.2f;

        public GameObject GameObject
            => this.gameObject;

        public int SphereNo { get; private set; }
        private int _maxSphereNo;

        private IInputEventProvider _inputEventProvider;
        private IDisposable _mouseMoveSubscription;
        private IDisposable _mouseClickSubscription;

        private readonly Subject<Unit> _onDropping
            = new Subject<Unit>();
        public IObservable<Unit> OnDropping
            => _onDropping;

        private readonly Subject<MergeData> _onMerging
            = new Subject<MergeData>();

        public IObservable<MergeData> OnMerging
            => _onMerging;

        private readonly ReactiveProperty<int> _nextSphereIndex = new();
        public IReadOnlyReactiveProperty<int> NextSphereIndex
            => _nextSphereIndex.ToReadOnlyReactiveProperty();

        private readonly ReactiveProperty<float> _ceilingContactTime = new();
        public IReadOnlyReactiveProperty<float> ContactTime
            => _ceilingContactTime.ToReadOnlyReactiveProperty();

        [Inject]
        public void Construct(
            [Inject(Id = "MaxItemNo")] int maxSphereNo,
            IInputEventProvider inputEventProvider)
        {
            _maxSphereNo = maxSphereNo;
            _inputEventProvider = inputEventProvider;
        }

        private void Awake()
        {
            _onDropping.AddTo(this);
            _onMerging.AddTo(this);
            _nextSphereIndex.AddTo(this);
            _ceilingContactTime.AddTo(this);

            _isDrop = false;
            _rb = GetComponent<Rigidbody2D>();
            _rb.simulated = false;
        }

        private void Start()
        {
            _mouseMoveSubscription = _inputEventProvider
                .OnMouseMove
                .Where(_ => !_isDrop && Time.timeScale != 0f)
                .Subscribe(UpdatePosition)
                .AddTo(this);

            _mouseClickSubscription = _inputEventProvider
                .OnMouseClick
                .Where(_ => !_isDrop && Time.timeScale != 0f)
                .Subscribe(_ => StartDropping())
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _inputEventProvider = null;
            _rb = null;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsEligibleForMerge(collision, out IMergeItemView otherSphere))
            {
                RequestMerge(otherSphere);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent<IGameOverTrigger>(out _))
                _ceilingContactTime.Value += Time.deltaTime;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<IGameOverTrigger>(out _))
                _ceilingContactTime.Value = 0;
        }

        public void Initialize(int sphereNo)
        {
            SphereNo = sphereNo;
        }

        public void InitializeAfterMerge(int sphereNo)
        {
            SphereNo = sphereNo;
            _isDrop = true;
        }

        private void UpdatePosition(Vector2 mousePos)
        {
            float currentDiameter = _minDiameter + _stepSize * (SphereNo + 1);
            float offset = currentDiameter / 2 + 0.01f;
            float adjustedMinX = _minX + offset;
            float adjustedMaxX = _maxX - offset;
            mousePos.x = Mathf.Clamp(mousePos.x, adjustedMinX, adjustedMaxX);
            mousePos.y = _fixedY;
            transform.position = mousePos;
        }

        private void StartDropping()
        {
            _onDropping.OnNext(Unit.Default);

            _isDrop = true;
            _rb.velocity = Vector2.zero;
            _rb.simulated = true;

            _mouseMoveSubscription?.Dispose();
            _mouseClickSubscription?.Dispose();
        }

        private bool IsEligibleForMerge(Collision2D collision, out IMergeItemView otherSphere)
        {
            otherSphere = null;
            GameObject colObj = collision.gameObject;
            if (!colObj.TryGetComponent<IMergeItemView>(out _))
                return false;

            otherSphere = colObj.GetComponent<IMergeItemView>();
            return SphereNo == otherSphere.SphereNo;
        }

        private void RequestMerge(IMergeItemView otherSphere)
        {
            if (gameObject.GetInstanceID() < otherSphere.GameObject.GetInstanceID() && SphereNo < _maxSphereNo - 1)
            {
                var newPosition = (transform.position + otherSphere.GameObject.transform.position) / 2;
                _onMerging.OnNext(new MergeData(newPosition, SphereNo, gameObject, otherSphere.GameObject));
            }
        }
    }
}