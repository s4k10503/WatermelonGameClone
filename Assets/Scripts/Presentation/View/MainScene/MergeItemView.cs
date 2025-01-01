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

        private bool _isDrop;

        // Physics
        public GameObject GameObject
            => gameObject;
        private Rigidbody2D _rb;
        private readonly float _minDiameter = 0.4f;
        private readonly float _stepSize = 0.2f;

        public int ItemNo { get; private set; }

        private IInputEventProvider _inputEventProvider;
        private IDisposable _mouseMoveSubscription;
        private IDisposable _mouseClickSubscription;

        private readonly Subject<Unit> _onDropping
            = new();
        public IObservable<Unit> OnDropping
            => _onDropping;

        private readonly Subject<(IMergeItemView Source, IMergeItemView Target)> _onMergeRequest
            = new();
        public IObservable<(IMergeItemView Source, IMergeItemView Target)> OnMergeRequest
            => _onMergeRequest;

        private readonly ReactiveProperty<int> _nextItemIndex = new();
        public IReadOnlyReactiveProperty<int> NextItemIndex
            => _nextItemIndex.ToReadOnlyReactiveProperty();

        private readonly ReactiveProperty<float> _ceilingContactTime = new();
        public IReadOnlyReactiveProperty<float> ContactTime
            => _ceilingContactTime.ToReadOnlyReactiveProperty();

        [Inject]
        public void Construct(
            IInputEventProvider inputEventProvider)
        {
            _inputEventProvider = inputEventProvider;
        }

        private void Awake()
        {
            _onDropping.AddTo(this);
            _onMergeRequest.AddTo(this);
            _nextItemIndex.AddTo(this);
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
            // If the collision target does not have iMergeiteMView, the process is terminated
            if (!collision.gameObject.TryGetComponent(out IMergeItemView targetItem))
                return;

            // If own instance id is larger than the other, the processing is terminated
            if (gameObject.GetInstanceID() >= targetItem.GameObject.GetInstanceID())
                return;

            // Send a merge request only when the conditions are met
            _onMergeRequest.OnNext((this, targetItem));
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

        public void Initialize(int itemNo)
        {
            ItemNo = itemNo;
        }

        public void InitializeAfterMerge(int itemNo)
        {
            ItemNo = itemNo;
            _isDrop = true;
        }

        private void UpdatePosition(Vector2 mousePos)
        {
            float currentDiameter = _minDiameter + _stepSize * (ItemNo + 1);
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
    }
}
