//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/Scripts/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""CharacterControl"",
            ""id"": ""f9d5600b-a2da-4df1-9441-9cc962403f58"",
            ""actions"": [
                {
                    ""name"": ""Pointer1"",
                    ""type"": ""Button"",
                    ""id"": ""7df80c99-cb0e-4c8b-bbbf-31740b7aa939"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pointer2"",
                    ""type"": ""Button"",
                    ""id"": ""919c3212-86c4-4cae-b193-41868fe67ab5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Halt"",
                    ""type"": ""Button"",
                    ""id"": ""1d6c173a-1d38-4cd4-a53a-63ba5fd2e14e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7b626e98-a86c-43a7-ab61-829b1b57b49c"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pointer1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ed93726-33be-4ae9-ada7-582e9881df1b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pointer2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77370b6c-6a89-4f11-bb76-5a68fb469f6f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Halt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""62ac1a59-c9ed-42a4-b834-f680b2596f58"",
            ""actions"": [
                {
                    ""name"": ""Confirm"",
                    ""type"": ""Button"",
                    ""id"": ""e0a47f08-56da-44af-ad91-73133bbe6865"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SkipDialog"",
                    ""type"": ""Button"",
                    ""id"": ""a562aabf-274e-4b86-bf7a-f0b376a34c96"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SpeedUpDialog"",
                    ""type"": ""Button"",
                    ""id"": ""254225e7-f56f-48ac-ac69-4b37b95381fe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""cb1dcf2a-8665-4b2e-8601-848ec2d6e64d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ShowInventory"",
                    ""type"": ""Button"",
                    ""id"": ""78d4f6d2-1d3d-456e-8e96-047f47305c1b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ce0182e6-c7f8-485e-af2a-d632958ad7c5"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc92bc6d-4b9e-499a-b909-b3e50b537b54"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SkipDialog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""074b7fcb-eb28-4009-a670-325e4af9e141"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SkipDialog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d99c6b8a-e55c-4044-84de-eef341b64623"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SkipDialog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53488578-9166-4c08-8963-69ca23dd5705"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpeedUpDialog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c191153d-d7df-4380-8c44-a5946a8b8a27"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpeedUpDialog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a89f5814-55e4-4a85-af61-fa1f6653378e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8afe3273-5966-48c8-ac77-4f8298f24b93"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShowInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""CameraControl"",
            ""id"": ""e3c8e18c-9e4d-4378-b5d6-a453c17d9392"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""451707a8-d120-4b6f-801f-f7498a36eae1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""c879439e-e85d-42e4-8742-988e825dd44e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""64fc5f5c-5109-4370-8caa-ecc6fd1147cd"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c7634ae5-15c3-4707-a8f0-c8f66635f36f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2c0383d7-2b81-4ecd-8125-51236d76fc43"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""06102e81-1509-4d0f-980f-3a3d106e67de"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Misc"",
            ""id"": ""684a652c-d952-4594-a5d2-daffb1726c8e"",
            ""actions"": [
                {
                    ""name"": ""TimeSpeedUp"",
                    ""type"": ""Button"",
                    ""id"": ""32d14890-0332-4140-bb61-1072dbb2ab20"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TimeSpeedDown"",
                    ""type"": ""Button"",
                    ""id"": ""fd237850-3a59-43b5-b29a-2c329c1a882a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""a70b9384-e175-4981-b517-7de94480985b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""GoBackToMenu"",
                    ""type"": ""Button"",
                    ""id"": ""5c66ba06-3603-4c62-9caa-ff121e8ff5e5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ab1fc891-ae3d-4e70-ac2d-036cea9f76dd"",
                    ""path"": ""<Keyboard>/equals"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeSpeedUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c782422-0365-4677-b128-6dd6b328e5b2"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeSpeedDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ab5bbb9-d055-4862-adec-3e6194b4bcca"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0dcd1ddf-d0a2-47aa-a165-a0695b2dd1fd"",
                    ""path"": ""<Keyboard>/f4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GoBackToMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard And Mouse"",
            ""bindingGroup"": ""Keyboard And Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // CharacterControl
        m_CharacterControl = asset.FindActionMap("CharacterControl", throwIfNotFound: true);
        m_CharacterControl_Pointer1 = m_CharacterControl.FindAction("Pointer1", throwIfNotFound: true);
        m_CharacterControl_Pointer2 = m_CharacterControl.FindAction("Pointer2", throwIfNotFound: true);
        m_CharacterControl_Halt = m_CharacterControl.FindAction("Halt", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Confirm = m_UI.FindAction("Confirm", throwIfNotFound: true);
        m_UI_SkipDialog = m_UI.FindAction("SkipDialog", throwIfNotFound: true);
        m_UI_SpeedUpDialog = m_UI.FindAction("SpeedUpDialog", throwIfNotFound: true);
        m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
        m_UI_ShowInventory = m_UI.FindAction("ShowInventory", throwIfNotFound: true);
        // CameraControl
        m_CameraControl = asset.FindActionMap("CameraControl", throwIfNotFound: true);
        m_CameraControl_Movement = m_CameraControl.FindAction("Movement", throwIfNotFound: true);
        // Misc
        m_Misc = asset.FindActionMap("Misc", throwIfNotFound: true);
        m_Misc_TimeSpeedUp = m_Misc.FindAction("TimeSpeedUp", throwIfNotFound: true);
        m_Misc_TimeSpeedDown = m_Misc.FindAction("TimeSpeedDown", throwIfNotFound: true);
        m_Misc_Pause = m_Misc.FindAction("Pause", throwIfNotFound: true);
        m_Misc_GoBackToMenu = m_Misc.FindAction("GoBackToMenu", throwIfNotFound: true);
    }

    ~@InputActions()
    {
        UnityEngine.Debug.Assert(!m_CharacterControl.enabled, "This will cause a leak and performance issues, InputActions.CharacterControl.Disable() has not been called.");
        UnityEngine.Debug.Assert(!m_UI.enabled, "This will cause a leak and performance issues, InputActions.UI.Disable() has not been called.");
        UnityEngine.Debug.Assert(!m_CameraControl.enabled, "This will cause a leak and performance issues, InputActions.CameraControl.Disable() has not been called.");
        UnityEngine.Debug.Assert(!m_Misc.enabled, "This will cause a leak and performance issues, InputActions.Misc.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // CharacterControl
    private readonly InputActionMap m_CharacterControl;
    private List<ICharacterControlActions> m_CharacterControlActionsCallbackInterfaces = new List<ICharacterControlActions>();
    private readonly InputAction m_CharacterControl_Pointer1;
    private readonly InputAction m_CharacterControl_Pointer2;
    private readonly InputAction m_CharacterControl_Halt;
    public struct CharacterControlActions
    {
        private @InputActions m_Wrapper;
        public CharacterControlActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pointer1 => m_Wrapper.m_CharacterControl_Pointer1;
        public InputAction @Pointer2 => m_Wrapper.m_CharacterControl_Pointer2;
        public InputAction @Halt => m_Wrapper.m_CharacterControl_Halt;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControlActions set) { return set.Get(); }
        public void AddCallbacks(ICharacterControlActions instance)
        {
            if (instance == null || m_Wrapper.m_CharacterControlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CharacterControlActionsCallbackInterfaces.Add(instance);
            @Pointer1.started += instance.OnPointer1;
            @Pointer1.performed += instance.OnPointer1;
            @Pointer1.canceled += instance.OnPointer1;
            @Pointer2.started += instance.OnPointer2;
            @Pointer2.performed += instance.OnPointer2;
            @Pointer2.canceled += instance.OnPointer2;
            @Halt.started += instance.OnHalt;
            @Halt.performed += instance.OnHalt;
            @Halt.canceled += instance.OnHalt;
        }

        private void UnregisterCallbacks(ICharacterControlActions instance)
        {
            @Pointer1.started -= instance.OnPointer1;
            @Pointer1.performed -= instance.OnPointer1;
            @Pointer1.canceled -= instance.OnPointer1;
            @Pointer2.started -= instance.OnPointer2;
            @Pointer2.performed -= instance.OnPointer2;
            @Pointer2.canceled -= instance.OnPointer2;
            @Halt.started -= instance.OnHalt;
            @Halt.performed -= instance.OnHalt;
            @Halt.canceled -= instance.OnHalt;
        }

        public void RemoveCallbacks(ICharacterControlActions instance)
        {
            if (m_Wrapper.m_CharacterControlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICharacterControlActions instance)
        {
            foreach (var item in m_Wrapper.m_CharacterControlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CharacterControlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CharacterControlActions @CharacterControl => new CharacterControlActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_Confirm;
    private readonly InputAction m_UI_SkipDialog;
    private readonly InputAction m_UI_SpeedUpDialog;
    private readonly InputAction m_UI_Cancel;
    private readonly InputAction m_UI_ShowInventory;
    public struct UIActions
    {
        private @InputActions m_Wrapper;
        public UIActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Confirm => m_Wrapper.m_UI_Confirm;
        public InputAction @SkipDialog => m_Wrapper.m_UI_SkipDialog;
        public InputAction @SpeedUpDialog => m_Wrapper.m_UI_SpeedUpDialog;
        public InputAction @Cancel => m_Wrapper.m_UI_Cancel;
        public InputAction @ShowInventory => m_Wrapper.m_UI_ShowInventory;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @Confirm.started += instance.OnConfirm;
            @Confirm.performed += instance.OnConfirm;
            @Confirm.canceled += instance.OnConfirm;
            @SkipDialog.started += instance.OnSkipDialog;
            @SkipDialog.performed += instance.OnSkipDialog;
            @SkipDialog.canceled += instance.OnSkipDialog;
            @SpeedUpDialog.started += instance.OnSpeedUpDialog;
            @SpeedUpDialog.performed += instance.OnSpeedUpDialog;
            @SpeedUpDialog.canceled += instance.OnSpeedUpDialog;
            @Cancel.started += instance.OnCancel;
            @Cancel.performed += instance.OnCancel;
            @Cancel.canceled += instance.OnCancel;
            @ShowInventory.started += instance.OnShowInventory;
            @ShowInventory.performed += instance.OnShowInventory;
            @ShowInventory.canceled += instance.OnShowInventory;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @Confirm.started -= instance.OnConfirm;
            @Confirm.performed -= instance.OnConfirm;
            @Confirm.canceled -= instance.OnConfirm;
            @SkipDialog.started -= instance.OnSkipDialog;
            @SkipDialog.performed -= instance.OnSkipDialog;
            @SkipDialog.canceled -= instance.OnSkipDialog;
            @SpeedUpDialog.started -= instance.OnSpeedUpDialog;
            @SpeedUpDialog.performed -= instance.OnSpeedUpDialog;
            @SpeedUpDialog.canceled -= instance.OnSpeedUpDialog;
            @Cancel.started -= instance.OnCancel;
            @Cancel.performed -= instance.OnCancel;
            @Cancel.canceled -= instance.OnCancel;
            @ShowInventory.started -= instance.OnShowInventory;
            @ShowInventory.performed -= instance.OnShowInventory;
            @ShowInventory.canceled -= instance.OnShowInventory;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);

    // CameraControl
    private readonly InputActionMap m_CameraControl;
    private List<ICameraControlActions> m_CameraControlActionsCallbackInterfaces = new List<ICameraControlActions>();
    private readonly InputAction m_CameraControl_Movement;
    public struct CameraControlActions
    {
        private @InputActions m_Wrapper;
        public CameraControlActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_CameraControl_Movement;
        public InputActionMap Get() { return m_Wrapper.m_CameraControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraControlActions set) { return set.Get(); }
        public void AddCallbacks(ICameraControlActions instance)
        {
            if (instance == null || m_Wrapper.m_CameraControlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CameraControlActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
        }

        private void UnregisterCallbacks(ICameraControlActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
        }

        public void RemoveCallbacks(ICameraControlActions instance)
        {
            if (m_Wrapper.m_CameraControlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICameraControlActions instance)
        {
            foreach (var item in m_Wrapper.m_CameraControlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CameraControlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CameraControlActions @CameraControl => new CameraControlActions(this);

    // Misc
    private readonly InputActionMap m_Misc;
    private List<IMiscActions> m_MiscActionsCallbackInterfaces = new List<IMiscActions>();
    private readonly InputAction m_Misc_TimeSpeedUp;
    private readonly InputAction m_Misc_TimeSpeedDown;
    private readonly InputAction m_Misc_Pause;
    private readonly InputAction m_Misc_GoBackToMenu;
    public struct MiscActions
    {
        private @InputActions m_Wrapper;
        public MiscActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @TimeSpeedUp => m_Wrapper.m_Misc_TimeSpeedUp;
        public InputAction @TimeSpeedDown => m_Wrapper.m_Misc_TimeSpeedDown;
        public InputAction @Pause => m_Wrapper.m_Misc_Pause;
        public InputAction @GoBackToMenu => m_Wrapper.m_Misc_GoBackToMenu;
        public InputActionMap Get() { return m_Wrapper.m_Misc; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MiscActions set) { return set.Get(); }
        public void AddCallbacks(IMiscActions instance)
        {
            if (instance == null || m_Wrapper.m_MiscActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MiscActionsCallbackInterfaces.Add(instance);
            @TimeSpeedUp.started += instance.OnTimeSpeedUp;
            @TimeSpeedUp.performed += instance.OnTimeSpeedUp;
            @TimeSpeedUp.canceled += instance.OnTimeSpeedUp;
            @TimeSpeedDown.started += instance.OnTimeSpeedDown;
            @TimeSpeedDown.performed += instance.OnTimeSpeedDown;
            @TimeSpeedDown.canceled += instance.OnTimeSpeedDown;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @GoBackToMenu.started += instance.OnGoBackToMenu;
            @GoBackToMenu.performed += instance.OnGoBackToMenu;
            @GoBackToMenu.canceled += instance.OnGoBackToMenu;
        }

        private void UnregisterCallbacks(IMiscActions instance)
        {
            @TimeSpeedUp.started -= instance.OnTimeSpeedUp;
            @TimeSpeedUp.performed -= instance.OnTimeSpeedUp;
            @TimeSpeedUp.canceled -= instance.OnTimeSpeedUp;
            @TimeSpeedDown.started -= instance.OnTimeSpeedDown;
            @TimeSpeedDown.performed -= instance.OnTimeSpeedDown;
            @TimeSpeedDown.canceled -= instance.OnTimeSpeedDown;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @GoBackToMenu.started -= instance.OnGoBackToMenu;
            @GoBackToMenu.performed -= instance.OnGoBackToMenu;
            @GoBackToMenu.canceled -= instance.OnGoBackToMenu;
        }

        public void RemoveCallbacks(IMiscActions instance)
        {
            if (m_Wrapper.m_MiscActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMiscActions instance)
        {
            foreach (var item in m_Wrapper.m_MiscActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MiscActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MiscActions @Misc => new MiscActions(this);
    private int m_KeyboardAndMouseSchemeIndex = -1;
    public InputControlScheme KeyboardAndMouseScheme
    {
        get
        {
            if (m_KeyboardAndMouseSchemeIndex == -1) m_KeyboardAndMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard And Mouse");
            return asset.controlSchemes[m_KeyboardAndMouseSchemeIndex];
        }
    }
    public interface ICharacterControlActions
    {
        void OnPointer1(InputAction.CallbackContext context);
        void OnPointer2(InputAction.CallbackContext context);
        void OnHalt(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnConfirm(InputAction.CallbackContext context);
        void OnSkipDialog(InputAction.CallbackContext context);
        void OnSpeedUpDialog(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnShowInventory(InputAction.CallbackContext context);
    }
    public interface ICameraControlActions
    {
        void OnMovement(InputAction.CallbackContext context);
    }
    public interface IMiscActions
    {
        void OnTimeSpeedUp(InputAction.CallbackContext context);
        void OnTimeSpeedDown(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnGoBackToMenu(InputAction.CallbackContext context);
    }
}
