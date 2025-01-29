using System;
using UnityEngine;
using UnityEngine.Animations;

namespace Managers
{
    [RequireComponent(typeof(ParentConstraint))]
    public class SelectionMarker : MonoBehaviour, IFreeable
    {
        public ParentConstraint ParentConstraint { get; private set; }
        public Creature Creature { get; set; }
        
        void Awake()
        {
            ParentConstraint = GetComponent<ParentConstraint>();
        }

        public void Deinitialize()
        {
            // Remove all sources
            for (int i = 0; i < ParentConstraint.sourceCount; i++)
            {
                ParentConstraint.RemoveSource(i);
            }
        }

        public void Initialize(Action free)
        {
        }
    }
}