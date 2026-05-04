using UnityEngine;
using DG.Tweening;

public class PlayerQuickSlotUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform slotParent;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeDuration = 0.5f;

    private SlotUI[] uiSlots;
    private Tween fadeTween;

    void Awake()
    {
        uiSlots = slotParent.GetComponentsInChildren<SlotUI>();
        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].SlotIndex = i;
            uiSlots[i].IsQuickSlot = true;
            uiSlots[i].IsInGameQuickSlot = true;
        }
        canvasGroup.alpha = 0;
    }

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnQuickSlotUpdated += () => RefreshUI(true);
            RefreshUI(false);
        }
    }

    void Update()
    {
        if (UIManager.Instance.IsAnyPopupOpen) return;

        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                HandleQuickSlotInput(i);
            }
        }
    }

    private void HandleQuickSlotInput(int index)
    {
        InventoryManager.Instance.UseItem(index, true, InventoryManager.Instance.Player);
        TriggerShow();
    }

    public void TriggerShow()
    {
        fadeTween?.Kill();

        canvasGroup.alpha = 1f; // 즉시 나타나게 함 (혹은 .DOFade(1f, 0.2f)로 부드럽게 등장 가능)

        fadeTween = canvasGroup.DOFade(0f, fadeDuration)
            .SetDelay(displayDuration) // 설정한 시간만큼 버틴 후
            .SetEase(Ease.InQuad);    // 서서히 사라지는 가속도 설정
    }

    private void RefreshUI(bool showEffect = true)
    {
        if (uiSlots == null) return;
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < InventoryManager.Instance.QuickSlots.Length)
            {
                uiSlots[i].UpdateSlot(InventoryManager.Instance.QuickSlots[i]);
            }
        }

        if (showEffect) TriggerShow();
    }
}