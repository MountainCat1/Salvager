using System;
using System.Linq;
using Items;
using Managers;
using UnityEngine;
using Zenject;

namespace ScriptableActions.Conditions
{
    public class PlayerHasItemCondition : ConditionBase
    {
        [Inject] IPlayerCharacterProvider _playerProvider;

        [SerializeField] private ItemBehaviour item;
        [SerializeField] private int amount = 1;

        private void Start()
        {
            if(item == null)
                Debug.LogError("Item is not set in PlayerHasItemCondition");
        }

        protected override bool Check()
        {
            var player = _playerProvider.Get();

            Debug.Log($"Checking if player has item {item.GetIdentifier()}");

            var itemAmount = player.Inventory.Items
                .Count(x => x
                    .GetIdentifier()
                    .Equals(item.GetIdentifier())
                );
            Debug.Log($"Player has {itemAmount} of {item.GetIdentifier()}");
            return itemAmount >= amount;
        }
    }
}