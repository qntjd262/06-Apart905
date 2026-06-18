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
    
    // [추가] 장착 명령을 내릴 PlayerEquip을 캐싱해둡니다.
    private PlayerEquip playerEquip; 

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
        playerEquip = FindFirstObjectByType<PlayerEquip>(); // 플레이어 장비 스크립트 찾기

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnQuickSlotUpdated += OnQuickSlotUpdatedCallback;
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

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnQuickSlotUpdated -= OnQuickSlotUpdatedCallback;
        }
    }

    private void OnQuickSlotUpdatedCallback()
    {
        if (this == null || canvasGroup == null) return;
        RefreshUI(true);
    }
    
    private void HandleQuickSlotInput(int index)
    {
        // [수정] UseItem 대신 단축키 번호에 맞춰 플레이어에게 장비를 장착시킵니다.
        if (playerEquip != null)
        {
            playerEquip.EquipFromQuickSlot(index);
        }

        // 단축키를 누른 퀵슬롯의 외곽선 하이라이트 켜기
        if (index < uiSlots.Length)
        {
            uiSlots[index].SelectSlot();
        }

        TriggerShow();
    }

    public void TriggerShow()
    {
        fadeTween?.Kill();
        canvasGroup.alpha = 1f;

        fadeTween = canvasGroup.DOFade(0f, fadeDuration)
            .SetDelay(displayDuration)
            .SetEase(Ease.InQuad);
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