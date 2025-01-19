using System;
using UnityEngine;
using Cinemachine;

namespace Managers
{
    public interface ICameraShakeService
    {
        void ShakeCamera(Vector2 position, float amount);
    }

    public class CameraShakeService : MonoBehaviour, ICameraShakeService
    {
        private CinemachineImpulseSource _impulseSource;
        
        [SerializeField] private float shakeMultiplier = 0.03f;
        

        private void Start()
        {
            if (Camera.main == null)
            {
                throw new NullReferenceException("Main camera is not found");
            }
            
            _impulseSource = Camera.main.GetComponent<CinemachineImpulseSource>();
        }

        // ShakeCamera method
        public void ShakeCamera(Vector2 position, float amount)
        {
            // Adjust the impulse source position and generate impulse
            _impulseSource.transform.position = position;
            _impulseSource.GenerateImpulse(amount * shakeMultiplier);
        }
    }
}