using DefaultNamespace;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Triggers
{
    public class ColliderTrigger : TriggerBase
    {
        [SerializeField] private PlayerDetector playerDetector;
        
        [Inject] IPlayerCharacterProvider _playerProvider;

        protected override void Start()
        {
            base.Start();
            
            playerDetector.OnPlayerEnter += HandleTriggerEnter;
        }

        private void HandleTriggerEnter()
        {
            RunActions();
        }
    }
}