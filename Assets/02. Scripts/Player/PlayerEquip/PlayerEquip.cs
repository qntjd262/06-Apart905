using System.Collections.Generic;
using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [Header("무기 장착 위치")]
    [SerializeField] private Transform equipPoint;

    private Dictionary<string, GameObject> instantiatedEquipItem = new Dictionary<string, GameObject>();
    public EquipItemData currentEquipItem { get; private set;}

    private GameObject currentEquipObject;
    private PlayerAttack playerAttack;
    public int currentQuickSlotIndex { get; private set; } = 0;

    private Headlight headlightSystem;

    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();

        if(playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        headlightSystem = GetComponent<Headlight>(); 
    }

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnQuickSlotUpdated += RefreshCurrentEquip;
            RefreshCurrentEquip(); 
        }
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnQuickSlotUpdated -= RefreshCurrentEquip;
        }
    }

    private void RefreshCurrentEquip()
    {
        EquipFromQuickSlot(currentQuickSlotIndex);
    }

    public void EquipItem(ItemData itemData)
    {
        if(currentEquipItem != null)
        {
            currentEquipObject.SetActive(false);
        }

        currentEquipItem = null;
        currentEquipObject = null;

        if (itemData == null || itemData.itemType != ItemType.Equipable)
        {
            Debug.Log("맨손");
            SetFlashlightAnimation(false);
            return;
        }

        EquipItemData equipData = itemData as EquipItemData;

        if(equipData == null || equipData.equipPrefab == null)
        {
            Debug.Log($"{itemData.Name}의 프리팹 데이터가 존재하지 않음");
            SetFlashlightAnimation(false);
            return;
        }

        #region 아이템 장착
        if (instantiatedEquipItem.ContainsKey(equipData.ID))
        {
            currentEquipObject = instantiatedEquipItem[equipData.ID];
            currentEquipObject.SetActive(true);
        }
        else
        {
            GameObject newEquip = Instantiate(equipData.equipPrefab, equipPoint);

            newEquip.transform.localPosition = Vector3.zero;
            newEquip.transform.localRotation = Quaternion.identity;

            instantiatedEquipItem.Add(equipData.ID, newEquip);
            currentEquipObject = newEquip;
        }

        currentEquipItem = equipData;
        Debug.Log($"{equipData.Name} 장착 완료");
        #endregion

        SetFlashlightAnimation(false);
    }

    public void UseCurrentItem()
    {
        if(currentEquipItem == null)
        {
            playerAttack.Attack();
            return;
        }

        if(playerAttack != null) playerAttack.Attack();
    }

    public void EquipFromQuickSlot(int slotIndex)
    {
        if(InventoryManager.Instance == null) return;

        currentQuickSlotIndex = slotIndex;
        InventorySlot slot = InventoryManager.Instance.QuickSlots[slotIndex];

        if(slot.IsEmpty || slot.item.itemType != ItemType.Equipable)
        {
            EquipItem(null);
        }
        else
        {
            EquipItem(slot.item);
        }
    }

    private void SetFlashlightAnimation(bool isHolding)
    {
        if(playerAnimator != null)
        {
            playerAnimator.SetBool("IsHoldFlashlight", isHolding);
        }
    }

    public bool TryAcquireHeadlight(ItemData itemData)
    {
        // 아이템의 이름이나 ID로 헤드라이트인지 체크 (기획에 맞게 이름을 수정하세요)
        if (itemData.Name == "Headlight" || itemData.Name == "헤드라이트")
        {
            if (headlightSystem != null)
            {
                headlightSystem.AcquireHeadlight();
            }

            if(QuestManager.Instance != null)
            {
                QuestManager.Instance.OnPickUp(itemData.Name, 1);
            }
            return true;
        }

        return false; 
    }
}