//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/ScriptableObject/InputActionAssets/PlayerDriverControls.inputactions
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

public partial class @PlayerDriverControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerDriverControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerDriverControls"",
    ""maps"": [
        {
            ""name"": ""Car"",
            ""id"": ""d552dd07-6bb8-4fe5-b1db-4b1c21ea28f8"",
            ""actions"": [
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""Button"",
                    ""id"": ""5e4e7ff4-19a6-4b0d-94f2-b5981acf050c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Vertical"",
                    ""type"": ""Button"",
                    ""id"": ""ba1b2b19-3642-45af-8b38-4ccf4673bc8d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""Button"",
                    ""id"": ""4d1d62a9-1c16-45cd-bd91-07f1e18baeca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""ADKey"",
                    ""id"": ""ec658532-1cd2-4d0d-a348-1d98b8d93fe0"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""04d56799-d3f6-429e-8967-31c4065f41e4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ce5e9dbe-614a-43d1-8d07-49e2d83ae76d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""SWKey"",
                    ""id"": ""8e612ce7-c1e7-4b83-b73e-b509ba4dfb7b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""700828e1-b06f-4786-8065-8dcff9ea37b1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""eebd38ee-5c7b-497a-98cb-23c31b94ecea"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7832cc65-dcef-4970-b149-766ba22e44ff"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Car
        m_Car = asset.FindActionMap("Car", throwIfNotFound: true);
        m_Car_Horizontal = m_Car.FindAction("Horizontal", throwIfNotFound: true);
        m_Car_Vertical = m_Car.FindAction("Vertical", throwIfNotFound: true);
        m_Car_Brake = m_Car.FindAction("Brake", throwIfNotFound: true);
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

    // Car
    private readonly InputActionMap m_Car;
    private List<ICarActions> m_CarActionsCallbackInterfaces = new List<ICarActions>();
    private readonly InputAction m_Car_Horizontal;
    private readonly InputAction m_Car_Vertical;
    private readonly InputAction m_Car_Brake;
    public struct CarActions
    {
        private @PlayerDriverControls m_Wrapper;
        public CarActions(@PlayerDriverControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal => m_Wrapper.m_Car_Horizontal;
        public InputAction @Vertical => m_Wrapper.m_Car_Vertical;
        public InputAction @Brake => m_Wrapper.m_Car_Brake;
        public InputActionMap Get() { return m_Wrapper.m_Car; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CarActions set) { return set.Get(); }
        public void AddCallbacks(ICarActions instance)
        {
            if (instance == null || m_Wrapper.m_CarActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CarActionsCallbackInterfaces.Add(instance);
            @Horizontal.started += instance.OnHorizontal;
            @Horizontal.performed += instance.OnHorizontal;
            @Horizontal.canceled += instance.OnHorizontal;
            @Vertical.started += instance.OnVertical;
            @Vertical.performed += instance.OnVertical;
            @Vertical.canceled += instance.OnVertical;
            @Brake.started += instance.OnBrake;
            @Brake.performed += instance.OnBrake;
            @Brake.canceled += instance.OnBrake;
        }

        private void UnregisterCallbacks(ICarActions instance)
        {
            @Horizontal.started -= instance.OnHorizontal;
            @Horizontal.performed -= instance.OnHorizontal;
            @Horizontal.canceled -= instance.OnHorizontal;
            @Vertical.started -= instance.OnVertical;
            @Vertical.performed -= instance.OnVertical;
            @Vertical.canceled -= instance.OnVertical;
            @Brake.started -= instance.OnBrake;
            @Brake.performed -= instance.OnBrake;
            @Brake.canceled -= instance.OnBrake;
        }

        public void RemoveCallbacks(ICarActions instance)
        {
            if (m_Wrapper.m_CarActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICarActions instance)
        {
            foreach (var item in m_Wrapper.m_CarActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CarActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CarActions @Car => new CarActions(this);
    public interface ICarActions
    {
        void OnHorizontal(InputAction.CallbackContext context);
        void OnVertical(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
    }
}
