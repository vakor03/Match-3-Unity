using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3._Scripts.Core
{
    public class InputReader : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;

        public Vector2 Selected => _playerInputActions.Player.Select.ReadValue<Vector2>();

        public event Action Fire;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Fire.performed += OnFire;
        }

        private void OnFire(InputAction.CallbackContext ctx)
        {
            Fire?.Invoke();
        }
        
        private void OnDestroy()
        {
            _playerInputActions.Player.Fire.performed -= OnFire;
            _playerInputActions.Disable();
        }
    }
}