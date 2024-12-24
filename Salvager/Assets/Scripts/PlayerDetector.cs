using System;
using Managers;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class PlayerDetector : MonoBehaviour
    {
        public Action OnPlayerEnter;

        [Inject] IPlayerCharacterProvider _playerProvider;

        [SerializeField] private ColliderEventProducer colliderEventProducer;

        public bool PlayerInside { get; private set; } = false;

        private void Start()
        {
            colliderEventProducer.TriggerEnter += HandleTriggerEnter;
            colliderEventProducer.TriggerExit += HandleTriggerExit;
        }

        private void HandleTriggerExit(Collider2D collision)
        {
            if (_playerProvider.IsPlayer(collision))
                PlayerInside = false;
        }

        private void HandleTriggerEnter(Collider2D collision)
        {
            if (_playerProvider.IsPlayer(collision))
            {
                PlayerInside = true;
                OnPlayerEnter?.Invoke();
            }
        }
    }
}