using System.Linq;
using Items;
using Managers;
using UnityEngine;
using Zenject;

namespace ScriptableActions
{
    public class RemovePlayerItemAction : ScriptableAction
    {
        [Inject] IPlayerCharacterProvider _playerProvider;
        
        [SerializeField] private ItemBehaviour item;
        
        public override void Execute()
        {
            base.Execute();
            
            var player = _playerProvider.Get();

            var itemToRemove = player.Inventory.GetItem(item.GetIdentifier());
            
            if(itemToRemove == null)
                Debug.LogError($"Item {item.GetIdentifier()} not found in player's inventory");
            
            player.Inventory.RemoveItem(itemToRemove);
        }
    }
}