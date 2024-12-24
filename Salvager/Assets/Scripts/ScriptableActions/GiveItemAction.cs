using Items;
using Managers;
using UnityEngine;
using Zenject;

namespace ScriptableActions
{
    public class GiveItemAction : ScriptableAction
    {
        [Inject] IPlayerCharacterProvider _playerProvider;
        
        [SerializeField] private ItemBehaviour item;
        
        public override void Execute() 
        {
            base.Execute();

            _playerProvider.Get().Inventory.AddItem(item);
        }
    }
}