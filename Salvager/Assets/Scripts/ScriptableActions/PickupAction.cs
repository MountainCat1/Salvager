using Items;
using Managers;
using UnityEngine;
using Zenject;

namespace ScriptableActions
{
    public class PickupAction : ScriptableAction
    {
        [SerializeField] private ItemBehaviour item;
        
        [Inject] private IPlayerCharacterProvider _playerCharacterProvider;
        
        public override void Execute()
        {
            base.Execute();
            
            var player = _playerCharacterProvider.Get();
            
            player.Inventory.AddItem(item);
            
            Destroy(gameObject);
        }

        public void SetItem(ItemBehaviour itemBehaviour)
        {
            if(itemBehaviour == null)
            {
                Debug.LogError("ItemBehaviour is null");
                return;
            }

            GetComponentInChildren<SpriteRenderer>().sprite = itemBehaviour.Icon;
            
            item = itemBehaviour;
        }
    }
}