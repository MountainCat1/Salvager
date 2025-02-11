using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Data
{
    public interface IDataResolver
    {
        Creature ResolveCreaturePrefab(CreatureData data);
        Sprite ResolveItemIcon(string itemIcon);
    }
    
    public class DataResolver : MonoBehaviour, IDataResolver
    {
        [SerializeField] private Creature creaturePrefab; 

        private Sprite[] _itemIcons = Array.Empty<Sprite>();
        private Creature[] _creatures = Array.Empty<Creature>();
        
        private void Start()
        {
            _creatures = Resources.LoadAll<Creature>("Creatures/");
            _itemIcons = Resources.LoadAll<Sprite>("Items/");
        }

        public Creature ResolveCreaturePrefab(CreatureData data)
        {
            return _creatures.FirstOrDefault(x => x.GetIdentifier() == data.CreatureID) ?? creaturePrefab;
        }
        
        public Sprite ResolveItemIcon(string itemIcon)
        {
            return _itemIcons.FirstOrDefault(x => x.name == itemIcon);
        }
    }

}