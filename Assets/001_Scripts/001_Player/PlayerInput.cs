using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        // 구르기 입력이 발생한 순간(눌린 프레임)에 호출되는 이벤트
        // PlayerDodge 등에서 구독하여 사용
        public event Action OnDodgeInput;

        // 다른 스크립트(PlayerMovement 등)가 읽어갈 현재 입력 방향
        public Vector2 MoveDirection { get; private set; }

        private PlayerControls controls;

        #region Unity Methods
        private void Awake()
        {
            controls = new PlayerControls();
        }

        private void OnEnable()
        {
            controls.Player.Enable();

            controls.Player.Dodge.performed += HandleDodgeInput;
        }

        private void OnDisable()
        {

            controls.Player.Dodge.performed -= HandleDodgeInput;

            controls.Player.Disable();
        }

        private void Update()
        {
            // Move 액션의 현재 값을 Vector2로 읽어옴
            // Composite(WASD)가 이미 정규화된 값을 반환하므로 별도 normalize 불필요
            MoveDirection = controls.Player.Move.ReadValue<Vector2>();
        }
        #endregion

        #region Input Action Callbacks
        private void HandleDodgeInput(InputAction.CallbackContext context)
        {
            OnDodgeInput?.Invoke();
        }
        #endregion
    }
}
