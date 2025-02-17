using Cinemachine;
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
    [SerializeField] private float scrollSensitive = 0.02f;

    private CinemachineVirtualCamera _camera;

    private void Start()
    {
        _inputManager.CameraMovement += OnCameraMovement;
        _camera = Camera.main.GetComponent<CinemachineVirtualCamera>();
        _inputManager.Zoom += OnZoom;
    }

    private void OnDestroy()
    {
        _inputManager.CameraMovement -= OnCameraMovement;
        _inputManager.Zoom -= OnZoom;
    }

    private void OnZoom(float zoom)
    {
        _camera.m_Lens.OrthographicSize = Mathf.Clamp(_camera.m_Lens.OrthographicSize - zoom * scrollSensitive, 5, 20);
    }

    private void OnCameraMovement(Vector2 move)
    {
        cameraParent.transform.position += new Vector3(move.x, move.y, 0) * Time.deltaTime * cameraSpeed * _camera.m_Lens.OrthographicSize;
    }

    public void MoveTo(Vector2 targetPosition)
    {
        cameraParent.transform.position = new Vector3(targetPosition.x, targetPosition.y, cameraParent.transform.position.z);
    }
}