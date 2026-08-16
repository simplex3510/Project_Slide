using UnityEngine;

namespace Combat
{
    public enum SkillBlockType
    {
        NormalAttack,   // 일반 공격 (스킬1)
        Skill2,
        Skill3,
        SpecialSkill    // 체이닝 불가. 자원(마나) 시스템 미구현으로 이번 프로토타입 스폰 대상에서 제외
    }

    // 블록 1개의 순수 데이터. 상태 머신이나 생명주기 로직을 갖지 않음.
    public class SkillBlock
    {
        public SkillBlockType Type { get; }

        public SkillBlock(SkillBlockType type)
        {
            Type = type;
        }
    }
}