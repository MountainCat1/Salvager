using System;
using Items;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "LootTable", menuName = "Custom/LootTable", order = 1)]
    public class LootTable : ScriptableObject
    {
        [SerializeField] private LootTableEntry[] entries;

        public ItemBehaviour GetRandomItem()
        {
            float totalWeight = 0;
            foreach (var entry in entries)
            {
                totalWeight += entry.weight;
            }

            float randomWeight = UnityEngine.Random.Range(0, totalWeight);
            foreach (var entry in entries)
            {
                randomWeight -= entry.weight;
                if (randomWeight <= 0)
                {
                    return entry.item;
                }
            }

            throw new Exception("No item found");
        }
    }
    
    [Serializable]
    public struct LootTableEntry
    {
        [SerializeField] public ItemBehaviour item;
        [SerializeField] public float weight;
    }
}