using System.Linq;
using Items;
using Managers;
using UnityEngine;
using Zenject;

namespace VictoryConditions
{
    [CreateAssetMenu(fileName = "VictoryCondition", menuName = "Custom/VictoryConditions/HaveItems")]
    public class HaveItemsCondition : VictoryCondition
    {
        [Inject] private ICreatureManager _creatureManager;

        [SerializeField] private ItemBehaviour item;
        [SerializeField] private int count = 3;

        [Header("Available variables: \n<count>\n<current_count>\n<item_name>")]
        [SerializeField] private string description = "Have <current_count>/<count> of item <item_name>";
        
        public override string GetDescription()
        {
            var creatures = _creatureManager.GetAliveCreatures();
            var itemCount = 0;
            
            foreach (var creature in creatures)
            {
                itemCount += creature.Inventory.Items
                    .Where(x => x.GetIdentifier() == item.GetIdentifier())
                    .Sum(x => x.Count);
            }
            
            return description
                .Replace("<count>", count.ToString())
                .Replace("<current_count>", itemCount.ToString())
                .Replace("<item_name>", item.Name);
        }

        public override bool Check()
        {
            var creatures = _creatureManager.GetAliveCreatures();

            var itemCount = 0;

            foreach (var creature in creatures)
            {
                itemCount += creature.Inventory.Items
                    .Where(x => x.GetIdentifier() == item.GetIdentifier())
                    .Sum(x => x.Count);
            }

            return itemCount >= count;
        }
    }
}