using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    // 블록 1개의 UI 표시 + 포인터 이벤트 수신 전담.
    // 자체 판단(검증/체이닝 로직)은 하지 않고, 발생한 이벤트를 그대로 DragInput에 위임하는 얇은 어댑터.
    public class SkillBlockView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
    {
        // 인덱스는 스스로 알아내지 않고 SkillBlockSlotView가 갱신 시점에 주입
        public SkillBlock Block { get; private set; }
        public int SlotIndex { get; private set; }

        private SkillBlockDragInput dragInput;

        private void Awake()
        {
            // 프로토타입 단계: 씬 내 단일 DragInput을 상위에서 찾아옴
            // (추후 SkillBlockSlotView가 직접 주입하는 방식으로 변경 가능)
            dragInput = GetComponentInParent<SkillBlockDragInput>();
        }

        // SkillBlockSlotView가 생성/동기화 시점에 호출
        public void Bind(SkillBlock block, int index)
        {
            Block = block;
            SlotIndex = index;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dragInput.BeginDrag(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            dragInput.ContinueDrag(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragInput.EndDrag();
        }
    }
}