using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f; // 임시 값, 추후 PlayerStats로 이전 예정

        private Rigidbody2D rb;
        private PlayerInput playerInput;

        private PlayerDodge playerDodge;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerInput = GetComponent<PlayerInput>();
            playerDodge = GetComponent<PlayerDodge>();

        }

        private void FixedUpdate()
        {
            // 구르기 중에는 PlayerDodge가 이동 제어권을 가지므로 일반 이동을 양보함
            if (playerDodge.IsDodging)
            {
                return;
            }

            Vector2 moveDirection = playerInput.MoveDirection;
            Vector2 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(targetPosition);

        }
    }
}