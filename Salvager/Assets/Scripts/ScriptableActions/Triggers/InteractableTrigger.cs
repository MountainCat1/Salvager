using System.Collections.Generic;
using Managers;
using UnityEngine;
using Zenject;

namespace Triggers
{
    public class InteractableTrigger : TriggerBase
    {
        [SerializeField] private float range = 1.5f;
    
        [Inject] private IPlayerCharacterProvider _playerProvider;
        
        private bool _hasInteracted = false;

        private void Update()
        {
            if(!CanRun)
                return;
            
            var player = _playerProvider.Get();

            if (Vector3.Distance(player.transform.position, transform.position) <= range)
            {
                if(_hasInteracted)
                    return;
                
                _hasInteracted = true;
                RunActions();
            }
            else
            {
                _hasInteracted = false;
            }
        }
    }

}