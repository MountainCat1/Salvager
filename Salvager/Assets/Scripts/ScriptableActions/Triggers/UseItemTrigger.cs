using Items;
using Managers;
using UnityEngine;
using Zenject;

namespace Triggers
{
    public class UseItemTrigger : TriggerBase
    {
        [SerializeField] private ItemBehaviour itemBehaviour;

        [Inject] IPlayerCharacterProvider _playerProvider;

        protected override void Start()
        {
            base.Start();

            _playerProvider.Get().Inventory.ItemUsed += HandleItemUsed;
        }

        private void HandleItemUsed(ItemBehaviour usedItem)
        {
            if (usedItem.GetIdentifier() != itemBehaviour.GetIdentifier())
                return;

            RunActions();
        }
    }
}