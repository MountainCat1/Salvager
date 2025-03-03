using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(ColliderEventProducer))]
    public class CollisionEventProducerDebugger : ColliderEventProducer
    {
        private void Start()
        {
            var producer = GetComponent<ColliderEventProducer>();
            producer.TriggerEnter += collider => Debug.Log($"TriggerEnter: {collider.gameObject.name}");
            producer.TriggerExit += collider => Debug.Log($"TriggerExit: {collider.gameObject.name}");
            producer.TriggerStay += collider => Debug.Log($"TriggerStay: {collider.gameObject.name}");
        }
    }
}