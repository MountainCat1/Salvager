using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ColliderEventProducer : MonoBehaviour
{
    public event Action<Collider2D> TriggerEnter;
    public event Action<Collider2D> TriggerExit;
    public event Action<Collider2D> TriggerStay;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter?.Invoke(other);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        TriggerExit?.Invoke(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TriggerStay?.Invoke(other);
    }
}