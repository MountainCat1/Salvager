using UnityEngine;

public class DestroyParticleSystemOnFinished : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        if (_particleSystem == null)
        {
            Debug.LogError(
                "DestroyParticleSystemOnFinished script requires a ParticleSystem component!");
            enabled = false; // Disable the script if no ParticleSystem is found
            return;
        }
        
    }

    void Update()
    {
        if (!_particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}