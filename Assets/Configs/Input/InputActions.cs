// GENERATED AUTOMATICALLY FROM 'Assets/Configs/Input/InputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActions : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""3a2607f5-52b4-4188-92ec-a92030a27d3b"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""cd4a73b8-fc8d-4df7-a576-af951165f1c7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LightAttack1"",
                    ""type"": ""Button"",
                    ""id"": ""306530b3-419f-4e00-9ed7-fa9d08bfc61c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LightAttack2"",
                    ""type"": ""Button"",
                    ""id"": ""dd3c1335-34e6-4c29-b78e-81efb68d877c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HeavyAttack1"",
                    ""type"": ""Button"",
                    ""id"": ""00e2251e-7ef3-40e0-9ed9-25f66d7e98ed"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HeavyAttack2"",
                    ""type"": ""Button"",
                    ""id"": ""2b2e19c1-d875-4b31-a4c6-e0975d8d8045"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LevelUp"",
                    ""type"": ""Button"",
                    ""id"": ""3c005ace-0afd-4e42-a6e5-9e484daea6ab"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Die"",
                    ""type"": ""Button"",
                    ""id"": ""65d4915d-d2d8-469a-a93c-34450b8cbd03"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""3c1d3337-057a-4660-ada2-65f61a1aa15f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""17cc8dfe-39c3-4ae6-8070-9e4ccf7ad5c3"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""95bd0ebf-200e-467d-aeef-c8d15133a7e7"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9206563c-0f68-468c-89c1-e40d7e11f6f2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""54f34a68-3cb5-4f3e-850e-7f59d50d7cba"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""14605974-562a-46f4-aaeb-e24efe4d33a9"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""LightAttack1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""312bea1b-5f31-4547-b35b-9f47f3b9dd52"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""LightAttack2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7228fa34-4370-4070-9349-0347ea024125"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""HeavyAttack1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3cad452e-5fb9-46a2-9245-712b8f84a412"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""HeavyAttack2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5b10134d-0ccb-465f-bf21-8c2c10d9b527"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""LevelUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""737e804e-8fcd-428c-b886-134decaab3f4"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Die"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_LightAttack1 = m_Player.FindAction("LightAttack1", throwIfNotFound: true);
        m_Player_LightAttack2 = m_Player.FindAction("LightAttack2", throwIfNotFound: true);
        m_Player_HeavyAttack1 = m_Player.FindAction("HeavyAttack1", throwIfNotFound: true);
        m_Player_HeavyAttack2 = m_Player.FindAction("HeavyAttack2", throwIfNotFound: true);
        m_Player_LevelUp = m_Player.FindAction("LevelUp", throwIfNotFound: true);
        m_Player_Die = m_Player.FindAction("Die", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_LightAttack1;
    private readonly InputAction m_Player_LightAttack2;
    private readonly InputAction m_Player_HeavyAttack1;
    private readonly InputAction m_Player_HeavyAttack2;
    private readonly InputAction m_Player_LevelUp;
    private readonly InputAction m_Player_Die;
    public struct PlayerActions
    {
        private @InputActions m_Wrapper;
        public PlayerActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @LightAttack1 => m_Wrapper.m_Player_LightAttack1;
        public InputAction @LightAttack2 => m_Wrapper.m_Player_LightAttack2;
        public InputAction @HeavyAttack1 => m_Wrapper.m_Player_HeavyAttack1;
        public InputAction @HeavyAttack2 => m_Wrapper.m_Player_HeavyAttack2;
        public InputAction @LevelUp => m_Wrapper.m_Player_LevelUp;
        public InputAction @Die => m_Wrapper.m_Player_Die;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @LightAttack1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLightAttack1;
                @LightAttack1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLightAttack1;
                @LightAttack1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLightAttack1;
                @LightAttack2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLightAttack2;
                @LightAttack2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLightAttack2;
                @LightAttack2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLightAttack2;
                @HeavyAttack1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavyAttack1;
                @HeavyAttack1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavyAttack1;
                @HeavyAttack1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavyAttack1;
                @HeavyAttack2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavyAttack2;
                @HeavyAttack2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavyAttack2;
                @HeavyAttack2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHeavyAttack2;
                @LevelUp.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLevelUp;
                @LevelUp.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLevelUp;
                @LevelUp.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLevelUp;
                @Die.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDie;
                @Die.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDie;
                @Die.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDie;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @LightAttack1.started += instance.OnLightAttack1;
                @LightAttack1.performed += instance.OnLightAttack1;
                @LightAttack1.canceled += instance.OnLightAttack1;
                @LightAttack2.started += instance.OnLightAttack2;
                @LightAttack2.performed += instance.OnLightAttack2;
                @LightAttack2.canceled += instance.OnLightAttack2;
                @HeavyAttack1.started += instance.OnHeavyAttack1;
                @HeavyAttack1.performed += instance.OnHeavyAttack1;
                @HeavyAttack1.canceled += instance.OnHeavyAttack1;
                @HeavyAttack2.started += instance.OnHeavyAttack2;
                @HeavyAttack2.performed += instance.OnHeavyAttack2;
                @HeavyAttack2.canceled += instance.OnHeavyAttack2;
                @LevelUp.started += instance.OnLevelUp;
                @LevelUp.performed += instance.OnLevelUp;
                @LevelUp.canceled += instance.OnLevelUp;
                @Die.started += instance.OnDie;
                @Die.performed += instance.OnDie;
                @Die.canceled += instance.OnDie;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLightAttack1(InputAction.CallbackContext context);
        void OnLightAttack2(InputAction.CallbackContext context);
        void OnHeavyAttack1(InputAction.CallbackContext context);
        void OnHeavyAttack2(InputAction.CallbackContext context);
        void OnLevelUp(InputAction.CallbackContext context);
        void OnDie(InputAction.CallbackContext context);
    }
}
