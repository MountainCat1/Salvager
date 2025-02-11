using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Data
{
    public interface IDataResolver
    {
        public Creature ResolveCreaturePrefab(CreatureData data);
    }
    
    public class DataResolver : MonoBehaviour, IDataResolver
    {
        [SerializeField] private Creature creaturePrefab;

        private Creature[] _creatures = Array.Empty<Creature>();
        
        private void Start()
        {
            _creatures = Resources.LoadAll<Creature>("Creatures/");
        }

        public Creature ResolveCreaturePrefab(CreatureData data)
        {
            return _creatures.FirstOrDefault(x => x.GetIdentifier() == data.CreatureID) ?? creaturePrefab;
        }
    }

}