using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public sealed class MergeItemView : MonoBehaviour, IMergeItemView
    {
        // Inspector Fields
        [Header("Movement restrictions")]
        [SerializeField] private float _minX = -2.7f;
        [SerializeField] private float _maxX = 2.7f;
        [SerializeField] private float _fixedY = 3.5f;

        // Constants & Private Fields
        private const float MinDiameter = 0.4f;
        private const float StepSize = 0.2f;

        private Rigidbody2D _rigidbody2D;
        private bool _isDropped;
        private Guid _id;

        private IInputEventProvider _inputEventProvider;
        private IDisposable _mouseMoveDisposable;
        private IDisposable _mouseClickDisposable;

        // Public Properties
        public GameObject GameObject => gameObject;
        public int ItemNo { get; private set; }

        // UniRx Subjects (Observables)
        private readonly Subject<Unit> _dropping = new();
        public IObservable<Unit> OnDropping => _dropping;

        private readonly Subject<(IMergeItemView Source, IMergeItemView Target)> _mergeRequest = new();
        public IObservable<(IMergeItemView Source, IMergeItemView Target)> OnMergeRequest => _mergeRequest;

        private readonly Subject<(Guid id, float deltaTime)> _contactTimeUpdated = new();
        public IObservable<(Guid id, float deltaTime)> OnContactTimeUpdated => _contactTimeUpdated;

        private readonly Subject<Guid> _contactExited = new();
        public IObservable<Guid> OnContactExited => _contactExited;

        [Inject]
        public void Construct(IInputEventProvider inputEventProvider)
        {
            _inputEventProvider = inputEventProvider;
        }

        private void Awake()
        {
            // Subscribe this instance's lifecycle to automatically dispose of Subjects
            _dropping.AddTo(this);
            _mergeRequest.AddTo(this);
            _contactTimeUpdated.AddTo(this);
            _contactExited.AddTo(this);

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.simulated = false;
            _isDropped = false;
        }

        private void Start()
        {
            // Submit the mouse movement to update the position
            _mouseMoveDisposable = _inputEventProvider
                .OnMouseMove
                .Where(_ => !_isDropped && Time.timeScale != 0f)
                .Subscribe(UpdatePosition)
                .AddTo(this);

            // Submit the mouse click to start drop processing
            _mouseClickDisposable = _inputEventProvider
                .OnMouseClick
                .Where(_ => !_isDropped && Time.timeScale != 0f)
                .Subscribe(_ => StartDropping())
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _inputEventProvider = null;
            _rigidbody2D = null;

            _mouseMoveDisposable?.Dispose();
            _mouseClickDisposable?.Dispose();
        }

        // Initialize new and synthesized items
        public void Initialize(Guid id, int itemNo, bool isAfterMerge = false)
        {
            _id = id;
            ItemNo = itemNo;
            _isDropped = isAfterMerge;

            if (isAfterMerge)
            {
                // After the merge, it is treated with the dropped and simulated the rigid body
                _mouseMoveDisposable?.Dispose();
                _mouseClickDisposable?.Dispose();
                _rigidbody2D.simulated = true;
            }
        }

        // Update the position of the item according to the mouse position
        private void UpdatePosition(Vector2 mousePos)
        {
            float currentDiameter = MinDiameter + StepSize * (ItemNo + 1);
            float offset = currentDiameter / 2 + 0.01f;

            float adjustedMinX = _minX + offset;
            float adjustedMaxX = _maxX - offset;

            mousePos.x = Mathf.Clamp(mousePos.x, adjustedMinX, adjustedMaxX);
            mousePos.y = _fixedY;

            transform.position = mousePos;
        }

        // Drop start processing
        private void StartDropping()
        {
            _dropping.OnNext(Unit.Default);

            _isDropped = true;
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.simulated = true;

            _mouseMoveDisposable?.Dispose();
            _mouseClickDisposable?.Dispose();
        }

        // Collision / Trigger Handlers
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // If the collision partner does not implement iMergeiteMView
            if (!collision.gameObject.TryGetComponent(out IMergeItemView targetItem)) return;

            // If your GetinstanceID () is larger than the collision partner, it will be processed first
            if (gameObject.GetInstanceID() >= targetItem.GameObject.GetInstanceID()) return;

            // Merge request notification
            _mergeRequest.OnNext((this, targetItem));
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Detect contact with the ceiling and notify the time
            if (collision.TryGetComponent<IGameOverTrigger>(out _))
            {
                _contactTimeUpdated.OnNext((_id, Time.deltaTime));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Detects contact release with the ceiling
            if (collision.TryGetComponent<IGameOverTrigger>(out _))
            {
                _contactExited.OnNext(_id);
            }
        }
    }
}
