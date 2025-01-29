using System.Linq;
using Items;
using UnityEngine;
using Zenject;

namespace Managers
{
    [CreateAssetMenu(fileName = "VictoryCondition", menuName = "Custom/VictoryConditions/HaveItems")]
    public class HaveItemsCondition : VictoryCondition
    {
        [Inject] private ICreatureManager _creatureManager;

        [SerializeField] private ItemBehaviour item;
        [SerializeField] private int count = 3;

        public override string GetDescription()
        {
            return $"Have {count} of item <b>{item.name}</b>";
        }

        public override bool Check()
        {
            var creatures = _creatureManager.GetCreatures();

            var itemCount = 0;

            foreach (var creature in creatures)
            {
                itemCount += creature.Inventory.Items.Count(x => x.GetIdentifier() == item.GetIdentifier());
            }


            return itemCount >= count;
        }
    }
}