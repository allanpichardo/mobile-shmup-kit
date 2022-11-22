using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

namespace ShmupKit.Controls
{
    public class InputManager : MonoBehaviour
    {
        private InputActions _inputActions;
        
        public UnityEvent<Vector3> OnMove;
        public UnityEvent<Vector2, Boolean> OnFire;

        private void Awake()
        {
            _inputActions = new InputActions();
        }
        
        private void OnEnable()
        {
            if (GravitySensor.current != null)
            {
                InputSystem.EnableDevice(GravitySensor.current);
            }
            
            _inputActions.Player.Enable();
            
            _inputActions.Player.Move.performed += OnMovePerformed;
            
            _inputActions.Player.Fire.performed += OnFirePerformed;
            _inputActions.Player.Fire.canceled += OnFireCanceled;
        }

        private void OnFireCanceled(InputAction.CallbackContext context)
        {
            OnFire?.Invoke(Vector2.zero, false);
        }

        private void OnFirePerformed(InputAction.CallbackContext context)
        {
            Vector2 screenPosition = context.ReadValue<Vector2>();
            OnFire?.Invoke(screenPosition, true);
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector3 angularVelocity = context.ReadValue<Vector3>();
            OnMove?.Invoke(angularVelocity);
        }
    }
}