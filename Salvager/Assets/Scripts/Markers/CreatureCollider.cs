using System;
using UnityEngine;

namespace Markers
{
    [RequireComponent(typeof(Collider2D))]
    public class CreatureCollider : MonoBehaviour
    {
        public Creature Creature { get; private set; }
        
        private void Awake()
        {
            Creature = GetComponentInParent<Creature>();
        }
    }
}