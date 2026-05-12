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

    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();

        if(playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();
    }

    public void EquipItem(ItemData itemData)
    {
        if(currentEquipItem != null)
        {
            currentEquipObject.SetActive(false);
        }

        currentEquipItem = null;
        currentEquipObject = null;

        //슬롯이 비어있거나, 장착형 아이템이 아닌경우 맨손 상태 종료 (현재 장착형 아이템이 아니면 퀵슬롯에 아이템이 들어올 수 없지만 추후 수정할 경우 필요)
        if(itemData == null || itemData.itemType != ItemType.Equipable)
        {
            Debug.Log("맨손");
            SetFlashlightAnimation(false);
            return;
        }

        //장착형 형변환 -> 프리팹
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
            //기존에 생성이 되었던 아이템인 경우, instantiatedEquipItem에서 가져와서 활성화
            currentEquipObject = instantiatedEquipItem[equipData.ID];
            currentEquipObject.SetActive(true);
        }
        else
        {
            //게임 시작 후 처음 장착하는 아이템인 경우 생성 후 instantiatedEquipItem에 추가
            GameObject newEquip = Instantiate(equipData.equipPrefab, equipPoint);

            newEquip.transform.localPosition = Vector3.zero;
            newEquip.transform.localRotation = Quaternion.identity;

            instantiatedEquipItem.Add(equipData.ID, newEquip);
            currentEquipObject = newEquip;
        }

        currentEquipItem = equipData;
        Debug.Log($"{equipData.Name} 장착 완료");
        #endregion

        bool isFlashlight = currentEquipObject.GetComponent<Flashlight>() != null;
        SetFlashlightAnimation(isFlashlight);
    }

    //현재 들고 있는 아이템에 따른 동작 분배기
    public void UseCurrentItem()
    {
        if(currentEquipItem == null)
        {
            playerAttack.Attack();
            return;
        }

        Flashlight flashlight = currentEquipObject.GetComponent<Flashlight>();

        if(flashlight != null)
        {
            //TODO : 손전등 사용 로직
            flashlight.ToggleFlashlight();
        }
        else
        {
            if(playerAttack != null) playerAttack.Attack();
        }

        
    }

    public void EquipFromQuickSlot(int slotIndex)
    {
        if(InventoryManager.Instance == null) return;

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
}
