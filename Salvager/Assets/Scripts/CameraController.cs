using UnityEngine;
using Zenject;

public interface ICameraController
{
    void MoveTo(Vector2 targetPosition);
}

public class CameraController : MonoBehaviour, ICameraController
{
    [Inject] IInputManager _inputManager;
    
    [SerializeField] private Transform cameraParent;
    [SerializeField] private float cameraSpeed = 5f;

    private void Start()
    {
        _inputManager.CameraMovement += OnCameraMovement;
    }

    private void OnCameraMovement(Vector2 move)
    {
        cameraParent.transform.position += new Vector3(move.x, move.y, 0) * Time.deltaTime * cameraSpeed;
    }

    public void MoveTo(Vector2 targetPosition)
    {
        cameraParent.transform.position = new Vector3(targetPosition.x, targetPosition.y, cameraParent.transform.position.z);
    }
}
