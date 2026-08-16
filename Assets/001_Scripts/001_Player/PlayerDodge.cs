using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerDodge : MonoBehaviour
    {
        // 구르기 진행 상태
        // Ready: 대기 중, 입력 받으면 Startup으로 전이
        private enum DodgeState
        {
            Ready,      // Ready: 대기 중, 입력 받으면 Startup으로 전이
            Startup,    // Startup: 선딜레이(Pre-Delay) 구간 (무적 아님)
            Active,     // Active: 실제 이동 + 무적 구간(iframe)
            Recovery    // Recovery: 후딜레이(Post-Delay) 구간 (무적 아님)
        }

        [Header("구르기 수치 (임시값, 추후 조정 예정)")]
        [SerializeField] private float dodgeDistance = 3f;
        [SerializeField] private float dodgeDuration = 0.3f; // 구르기 전체 지속 시간
        [SerializeField] private float cooldown = 0.8f;

        [Header("구간 비율 (Startup + Active + Recovery = 1)")]
        [Range(0f, 1f)][SerializeField] private float startupRatio = 0.15f;
        [Range(0f, 1f)][SerializeField] private float activeRatio = 0.65f;
        [Range(0f, 1f)][SerializeField] private float recoveryRatio = 0.20f;


        [Header("시각적 피드백 (임시 - 추후 애니메이션으로 대체 예정)")]
        [SerializeField] private Color invincibleColor = new Color(1f, 1f, 1f, 0.5f);
        // 반투명으로 무적 표현


        private Rigidbody2D rb;
        private PlayerInput playerInput;
        private PlayerMovement playerMovement;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        private DodgeState currentState = DodgeState.Ready;
        private float stateTimer;      // 현재 상태(Startup/Active/Recovery)에 머문 시간
        private float cooldownTimer;   // 쿨다운 남은 시간
        private Vector2 dodgeDirection; // 이번 구르기에 사용할 고정 방향

        // 구르기 전체 지속시간을 비율에 따라 나눈 구간별 실제 시간(초)
        private float StartupDuration => dodgeDuration * startupRatio;
        private float ActiveDuration => dodgeDuration * activeRatio;
        private float RecoveryDuration => dodgeDuration * recoveryRatio;

        // 다른 스크립트(Hurtbox 등)가 참조할 수 있는 무적 여부
        public bool IsInvincible => currentState == DodgeState.Active;

        // PlayerMovement가 일반 이동을 양보해야 하는지 여부
        public bool IsDodging => currentState != DodgeState.Ready;

        #region Unity Methods
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerInput = GetComponent<PlayerInput>();
            playerMovement = GetComponent<PlayerMovement>();

            // Root/Graphic에 위치한 SpriteRenderer 참조
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        private void OnEnable()
        {
            playerInput.OnDodgeInput += HandleDodgeInput;
        }

        private void OnDisable()
        {
            playerInput.OnDodgeInput -= HandleDodgeInput;
        }

        private void FixedUpdate()
        {
            if (currentState == DodgeState.Ready)
            {
                return;
            }

            stateTimer += Time.fixedDeltaTime;

            switch (currentState)
            {
                case DodgeState.Startup:
                    if (stateTimer >= StartupDuration)
                    {
                        TransitionTo(DodgeState.Active);
                    }
                    break;

                case DodgeState.Active:
                    MoveDuringDodge();
                    if (stateTimer >= ActiveDuration)
                    {
                        TransitionTo(DodgeState.Recovery);
                    }
                    break;

                case DodgeState.Recovery:
                    if (stateTimer >= RecoveryDuration)
                    {
                        TransitionTo(DodgeState.Ready);
                    }
                    break;
            }
        }

        private void Update()
        {
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
        }
        #endregion

        private void TransitionTo(DodgeState nextState)
        {
            currentState = nextState;
            stateTimer = 0f;
            
            UpdateInvincibilityVisual();
        }

        private void UpdateInvincibilityVisual()
        {
            spriteRenderer.color = IsInvincible ? invincibleColor : originalColor;
        }


        private void MoveDuringDodge()
        {
            // 속도(Speed) = 거리(Distance) / 시간(Duration)
            float dodgeSpeed = dodgeDistance / ActiveDuration;
            Vector2 targetPosition = rb.position + dodgeDirection * dodgeSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }

        private void HandleDodgeInput()
        {
            if (!CanDodge())
            {
                return;
            }

            StartDodge();
        }

        private bool CanDodge()
        {
            return currentState == DodgeState.Ready && cooldownTimer <= 0f;
        }

        private void StartDodge()
        {
            // 방향 결정: 이동 입력 중이면 현재 입력 방향, 정지 상태면 Facing 방향
            Vector2 rawDirection = playerInput.MoveDirection != Vector2.zero
                ? playerInput.MoveDirection
                : playerInput.FacingDirection;
            dodgeDirection = rawDirection.normalized;

            currentState = DodgeState.Startup;
            stateTimer = 0f;
            cooldownTimer = cooldown;
        }
    }
}