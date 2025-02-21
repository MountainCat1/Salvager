using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Newtonsoft.Json;
using Pathfinding.Collections;
using UnityEngine;

namespace Data
{
    public enum ItemType
    {
        Default,
        Weapon,
        Consumable,
        Armor,
        Quest,
        Junk
    }

    [Serializable]
    public class ItemData
    {
        [HideInInspector]
        public string Identifier;
        
        public string Name;
        public int Count = 1;
        public bool Stackable;
        public decimal Value;
        public ItemType Type;
        public List<ItemModifier> Modifiers = new List<ItemModifier>();

        public static ItemData FromItemPrefab(ItemBehaviour item)
        {
            if(!item.IsPrefab)
                throw new ArgumentException("Item must be a prefab");

            var data = item.ItemData;

            data.Count = 1;
            data.Stackable = item.Stackable;
            
            return data;
        }
    }
    
    [Serializable]
    public class WeaponData : ItemData
    {
        public float BaseDamage;
        public float BaseAttackSpeed;
        public float Range;
     
        public WeaponData()
        {
            Type = ItemType.Weapon;
        }
        
        public float GetDamage()
        {
            return BaseDamage + Modifiers.OfType<WeaponValueModifier>().Where(m => m.Name == WeaponPropertyModifiers.Damage).Sum(m => m.Value);
        }
    }
    

    public enum WeaponPropertyModifiers
    {
        Damage,
        AttackSpeed,
        Range,
    }


    public class ItemModifier
    {
    }


    public class WeaponValueModifier : ItemModifier
    {
        public WeaponPropertyModifiers Name;
        public float Value;
        
        public static float GetApplied(IEnumerable<WeaponValueModifier> modifiers, WeaponPropertyModifiers property, float baseValue)
        {
            return baseValue + GetChange(modifiers, property, baseValue);
        }

        public static float GetChange(IEnumerable<WeaponValueModifier> modifiers, WeaponPropertyModifiers property, float baseValue)
        {
            var percentageModifier = modifiers
                .Where(m => m.Name == property)
                .Sum(x => x.Value);

            if (percentageModifier <= -1)
            {
                return -baseValue;
            }
            
            return baseValue * percentageModifier;
        }
    }
    
    public class WeaponSpecialModifier : ItemModifier
    {
        public string Identifier;
    }
}