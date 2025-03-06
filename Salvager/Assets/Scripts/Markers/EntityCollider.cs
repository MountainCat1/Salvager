using UnityEngine;

namespace Markers
{
    [RequireComponent(typeof(Collider2D))]
    public class EntityCollider : MonoBehaviour
    {
        public Entity Entity { get; private set; }
        
        protected virtual void Awake()
        {
            Entity = GetComponent<Creature>() 
                       ?? GetComponentInParent<Creature>() 
                       ?? throw new MissingComponentException("EntityCollider requires a Creature component");
        }
    }
}