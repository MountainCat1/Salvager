using Managers;
using UnityEngine;
using Zenject;

namespace ScriptableActions
{
    public class TeleportPlayer : ScriptableAction
    {
        [SerializeField] private Transform targetTransform;

        [Inject] private IPlayerCharacterProvider _playerProvider;
        
        public override void Execute()
        {
            base.Execute();
            
            var player = _playerProvider.Get();
            player.transform.position = targetTransform.position;
        }
    }
}