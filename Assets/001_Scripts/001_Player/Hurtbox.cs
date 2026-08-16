using System;
using UnityEngine;

namespace Player
{
    // Root/Hurtbox 오브젝트에 부착. 피해를 "받는" 판정 전담.
    // 실제 데미지 적용 로직(체력 감소 등)은 담당하지 않고, 피격이 확정되면
    // OnHit 이벤트만 발생시킴. PlayerStats 등 체력 시스템이 이 이벤트를 구독해서 처리할 예정.
    [RequireComponent(typeof(Collider2D))]
    public class Hurtbox : MonoBehaviour
    {
        // 피격이 확정된 순간(무적이 아닐 때) 발생. 인자는 부딪힌 상대 Collider2D.
        public event Action<Collider2D> OnHit;

        private PlayerDodge playerDodge;

        private void Awake()
        {
            // Hurtbox는 Root의 자식 오브젝트이므로, 부모에서 PlayerDodge를 찾아옴
            playerDodge = GetComponentInParent<PlayerDodge>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 무적 상태(구르기 Active 구간)면 피격을 무시
            if (playerDodge != null && playerDodge.IsInvincible)
            {
                return;
            }

            // TODO: 적 Hitbox 레이어/태그 필터링 (지금은 모든 트리거 진입을 피격으로 간주)
            // Hitbox 시스템(스킬/적 공격 판정) 확정 시 필터 조건 추가 예정
            OnHit?.Invoke(other);
        }
    }
}