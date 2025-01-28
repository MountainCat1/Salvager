using System;
using System.Linq;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementComponent : MonoBehaviour
    {
        public event Action<Vector2> Moved;

        [field: SerializeField]
        public float Drag { get; private set; }

        [field: SerializeField]
        public float BaseSpeed { get; set; }

        // Accessors
        public Vector2 Velocity => _rigidbody2D.velocity;
        public Vector2 MoveDirection => _moveDirection;
        public CircleCollider2D Collider => _collider2D;

        // References
        private Rigidbody2D _rigidbody2D;
        
        // Private Variables
        private Vector2 _moveDirection;
        private Vector2 _momentum;
        private CircleCollider2D _collider2D;

        private const float MomentumLoss = 2f;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = transform.GetComponentsInChildren<CircleCollider2D>()
                .Single(x => x.gameObject.layer == LayerMask.NameToLayer("CreatureMovement"));
        }

        private void FixedUpdate()
        {
            UpdateVelocity();
        }

        public void SetMovement(Vector2 direction)
        {
            _moveDirection = direction.normalized;
        }

        public void Push(Vector2 push)
        {
            _momentum = push;
        }

        private void UpdateVelocity()
        {
            _momentum -= _momentum * (MomentumLoss * Time.fixedDeltaTime);
            if (_momentum.magnitude < 0.1f)
                _momentum = Vector2.zero;

            var targetVelocity = _moveDirection * BaseSpeed + _momentum;
            var change = Vector2.MoveTowards(_rigidbody2D.velocity, targetVelocity, Drag * Time.fixedDeltaTime);
            _rigidbody2D.velocity = change;

            Moved?.Invoke(change);
        }
    }
}