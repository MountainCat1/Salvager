using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public interface IInputManager
{
    event Action<Vector2> CameraMovement;
    event Action<Vector2> Pointer1Pressed;
    event Action<Vector2> Pointer2Pressed;
    public event Action<Vector2> Pointer1Hold;
    event Action OnConfirm;
    event Action OnSpeedUpDialog;
    event Action OnSkip;
    event Action OnCancel;

    event Action SpeedUp;
    event Action SpeedDown;
}

public class InputManager : MonoBehaviour, IInputManager
{
    public event Action<Vector2> CameraMovement;
    public event Action<Vector2> Pointer1Pressed;
    public event Action<Vector2> Pointer2Pressed;
    public event Action<Vector2> Pointer1Hold;
    public event Action OnConfirm;
    public event Action OnSpeedUpDialog;
    public event Action OnSkip;
    public event Action OnCancel;
    public event Action SpeedUp;
    public event Action SpeedDown;

    [SerializeField] private int uiLayer = 5;

    private InputActions _inputActions;
    private Vector2 _cachedCharacterMovement = Vector2.zero;

    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _inputActions.CameraControl.Movement.performed +=
            ctx => CameraMovement?.Invoke(ctx.ReadValue<Vector2>());

        _inputActions.CharacterControl.Pointer1.performed += Pointer1OnPerformed;
        _inputActions.CharacterControl.Pointer2.performed += Pointer2OnPerformed;

        _inputActions.UI.Confirm.performed += _ => OnConfirm?.Invoke();
        _inputActions.UI.SkipDialog.performed += _ => OnSkip?.Invoke();
        _inputActions.UI.SpeedUpDialog.performed += _ => OnSpeedUpDialog?.Invoke();
        
        _inputActions.UI.Cancel.performed += _ => OnCancel?.Invoke();
        
        _inputActions.Misc.TimeSpeedUp.performed += _ => SpeedUp?.Invoke();
        _inputActions.Misc.TimeSpeedDown.performed += _ => SpeedDown?.Invoke();
    }

    private void Update()
    {
        if(_inputActions.CameraControl.Movement.IsPressed())
        {
            CameraMovement?.Invoke(_inputActions.CameraControl.Movement.ReadValue<Vector2>());
        }
        
        if (_inputActions.CharacterControl.Pointer1.IsPressed())
        {
            Pointer1Hold?.Invoke(Mouse.current.position.ReadValue());
        }
    }

    private void Pointer1OnPerformed(InputAction.CallbackContext callback)
    {
        if (IsPointerOverUI()) return;

        var pointerPosition = Mouse.current.position;
        Pointer1Pressed?.Invoke(pointerPosition.ReadValue());
    }

    private void Pointer2OnPerformed(InputAction.CallbackContext callback)
    {
        if (IsPointerOverUI()) return;

        var pointerPosition = Mouse.current.position;
        Pointer2Pressed?.Invoke(pointerPosition.ReadValue());
    }

    private bool IsPointerOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Mouse.current.position.ReadValue();
        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        for (int i = 0; i < raycastResultsList.Count; i++)
        {
            var raycastResult = raycastResultsList[i];
            if (raycastResult.gameObject.transform is RectTransform && raycastResult.gameObject.layer == uiLayer)
            {
                return true;
            }
        }

        return false;
    }
}