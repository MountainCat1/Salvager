using System;
using Data;
using UnityEngine;

namespace Items
{
    public struct AbilityUseContext
    {
        public Creature User { get; }
        public Creature Target { get; }
        public Vector2 TargetPosition { get; }
        
        public AbilityUseContext(Creature user, Creature target, Vector2 targetPosition)
        {
            User = user;
            Target = target;
            TargetPosition = targetPosition;
        }
    }
    
    public class Ability
    {
        public string Identifier { get; }
        
        public Ability(string identifier)
        {
            Identifier = identifier;
        }
    }
    
    public class ActiveItemBehaviour : ItemBehaviour
    {
        [field: SerializeField] public override ItemData ItemData { get; protected set; }
        
        public Ability GetAbility()
        {
            return new Ability($"{GetIdentifier()}_ability");
        }

        public virtual void UseActiveAbility(AbilityUseContext context)
        {
            Debug.Log($"Using ability {GetAbility().Identifier} on {context.Target?.name ?? "position " + context.TargetPosition}");
        }


    }
}