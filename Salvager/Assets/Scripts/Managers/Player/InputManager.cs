using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputManager
{
    event Action<Vector2> CameraMovement;
    event Action<Vector2> Pointer1Pressed;
    event Action<Vector2> Pointer1Released;
    event Action<Vector2> Pointer2Pressed;
    event Action Halt;
    public event Action<Vector2> Pointer1Hold;
    event Action OnConfirm;
    event Action OnSpeedUpDialog;
    event Action OnSkip;
    event Action OnCancel;
    event Action<float> Zoom;

    event Action SpeedUp;
    event Action SpeedDown;
    event Action Pause;
    event Action GoBackToMenu;
    IUIEvents UI { get; }
    bool IsShiftPressed { get; }
}

public interface IUIEvents
{
    public event Action ShowInventory;
    public event Action GoBack;
}

public class UIEvents : IUIEvents
{
    public event Action ShowInventory;
    public event Action GoBack;

    public UIEvents(InputActions inputActions)
    {
        inputActions.UI.ShowInventory.performed += _ => ShowInventory?.Invoke();
        inputActions.UI.GoBack.performed += _ => GoBack?.Invoke();
    }
}

public class InputManager : MonoBehaviour, IInputManager
{
    public event Action<Vector2> CameraMovement;
    public event Action<Vector2> Pointer1Pressed;
    public event Action<Vector2> Pointer1Released;
    public event Action<Vector2> Pointer2Pressed;
    public event Action Halt;
    public event Action<Vector2> Pointer1Hold;
    public event Action OnConfirm;
    public event Action OnSpeedUpDialog;
    public event Action OnSkip;
    public event Action OnCancel;
    public event Action<float> Zoom;
    public event Action SpeedUp;
    public event Action SpeedDown;
    public event Action Pause;
    public event Action GoBackToMenu;
    public IUIEvents UI { get; private set; }
    public bool IsShiftPressed => Keyboard.current.shiftKey.isPressed;

    private InputActions _inputActions;

    private void OnEnable()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();

        _inputActions.CameraControl.Movement.performed +=
            ctx => CameraMovement?.Invoke(ctx.ReadValue<Vector2>());

        _inputActions.CharacterControl.Pointer1.performed += Pointer1OnPerformed;
        _inputActions.CharacterControl.Pointer1.canceled += Pointer1Canceled;
        _inputActions.CharacterControl.Pointer2.performed += Pointer2OnPerformed;
        _inputActions.CharacterControl.Halt.performed += _ => Halt?.Invoke();

        _inputActions.Misc.TimeSpeedDown.performed += _ => SpeedDown?.Invoke();
        _inputActions.Misc.TimeSpeedUp.performed += _ => SpeedUp?.Invoke();
        _inputActions.Misc.Pause.performed += _ => Pause?.Invoke();
        _inputActions.Misc.GoBackToMenu.performed += _ => GoBackToMenu?.Invoke();
        
        _inputActions.UI.Confirm.performed += _ => OnConfirm?.Invoke();
        _inputActions.UI.SkipDialog.performed += _ => OnSkip?.Invoke();
        _inputActions.UI.SpeedUpDialog.performed += _ => OnSpeedUpDialog?.Invoke();

        _inputActions.UI.Cancel.performed += _ => OnCancel?.Invoke();
        
        _inputActions.UI.ZoomIn.performed += ctx => Zoom?.Invoke(ctx.ReadValue<float>());
        _inputActions.UI.ZoomOut.performed += ctx => Zoom?.Invoke(-ctx.ReadValue<float>());
        
        UI = new UIEvents(_inputActions);
    }

    private void Pointer1Canceled(InputAction.CallbackContext obj)
    {
        Pointer1Released?.Invoke(Mouse.current.position.ReadValue());
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
        var pointerPosition = Mouse.current.position;
        Pointer1Pressed?.Invoke(pointerPosition.ReadValue());
    }

    private void Pointer2OnPerformed(InputAction.CallbackContext callback)
    {
        var pointerPosition = Mouse.current.position;
        Pointer2Pressed?.Invoke(pointerPosition.ReadValue());
    }
}