using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;

    public int SlotIndex { get; set; }
    public bool IsQuickSlot { get; set; }
    public bool IsStorageSlot { get; set; }
    public bool IsInGameQuickSlot { get; set; }

    private static SlotUI pickedSlot;
    public static SlotUI PickedSlot => pickedSlot;
    private static Image cursorIcon;  // 마우스를 따라다닐 아이콘

    public void UpdateSlot(InventorySlot slotData)
    {
        if (slotData.IsEmpty)
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }
        else
        {
            // 현재 이 슬롯이 아이템을 "들고 있는" 슬롯이라면 아이콘을 반투명하게
            bool isPicked = (pickedSlot == this);
            icon.sprite = slotData.item.icon;
            icon.color = new Color(1, 1, 1, isPicked ? 0.3f : 1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsInGameQuickSlot) return;

        // 오른쪽 클릭 (기존 메뉴 로직)
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleRightClick();
            return;
        }

        // 왼쪽 클릭 (Pick & Place 로직)
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 1. 아무것도 들고 있지 않을 때 -> 아이템 들어올리기
            if (pickedSlot == null)
            {
                if (icon.sprite == null) return; // 빈 칸 클릭 무시

                PickUpItem();
            }
            // 2. 이미 아이템을 들고 있을 때 -> 여기에 내려놓기(Swap)
            else
            {
                PlaceItem();
            }
        }
    }

    private void PickUpItem()
    {
        pickedSlot = this;

        // 마우스 커서 아이콘 생성 (없다면)
        if (cursorIcon == null)
        {
            GameObject iconObj = new GameObject("CursorIcon");
            iconObj.transform.SetParent(GetComponentInParent<Canvas>().transform);
            cursorIcon = iconObj.AddComponent<Image>();
            cursorIcon.raycastTarget = false; // 마우스 클릭 방해 금지
            cursorIcon.rectTransform.sizeDelta = new Vector2(50, 50); // 적절한 크기
        }

        cursorIcon.sprite = icon.sprite;
        cursorIcon.gameObject.SetActive(true);

        // 원본 슬롯 반투명화
        icon.color = new Color(1, 1, 1, 0.3f);
    }

    private void PlaceItem()
    {
        // 로직은 기존 OnDrop과 동일하지만 pickedSlot을 사용
        ExecuteSwap(pickedSlot, this);

        // 들고 있는 상태 해제
        pickedSlot = null;
        if (cursorIcon != null) cursorIcon.gameObject.SetActive(false);

        // 모든 슬롯의 UI 갱신 (반투명 해제 등을 위해)
        // 실제 프로젝트에서는 이벤트나 매니저를 통해 호출하는 것이 좋음
        InventoryManager.Instance.OnBagUpdated?.Invoke();
        InventoryManager.Instance.OnStorageUpdated?.Invoke();
        InventoryManager.Instance.OnQuickSlotUpdated?.Invoke();
    }

    private void Update()
    {
        if (pickedSlot == this && cursorIcon != null)
        {
            // 1. 아이콘이 마우스를 따라다님
            cursorIcon.transform.position = Input.mousePosition;

            // 2. ESC 키 감지 시 취소
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPick();
            }

            // 3. 슬롯이 아닌 빈 공간 클릭 시 취소
            // 마우스 왼쪽 클릭이 눌렸는데, 마우스 아래에 UI 객체가 없으면 빈 공간으로 판단
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    CancelPick();
                }
            }

            // 4. 인벤토리 자체가 비활성화될 때를 대비 (OnDisable에서도 처리 가능)
            if (!gameObject.activeInHierarchy)
            {
                CancelPick();
            }
        }
    }

    public void CancelPick()
    {
        if (pickedSlot == null) return;

        pickedSlot = null;
        if (cursorIcon != null) cursorIcon.gameObject.SetActive(false);

        InventoryManager.Instance.OnBagUpdated?.Invoke();
        InventoryManager.Instance.OnStorageUpdated?.Invoke();
        InventoryManager.Instance.OnQuickSlotUpdated?.Invoke();
    }

    private void OnDisable()
    {
        if (pickedSlot == this)
        {
            CancelPick();
        }
    }

    private void ExecuteSwap(SlotUI from, SlotUI to)
    {
        if (from == to) return;

        // 기존 OnDrop에 있던 스왑 조건문 로직을 그대로 사용
        if (from.IsQuickSlot || to.IsQuickSlot)
        {
            if (from.IsStorageSlot || to.IsStorageSlot) return;

            if (!from.IsQuickSlot && to.IsQuickSlot)
                InventoryManager.Instance.SwapItemBetweenBagAndQuickSlot(from.SlotIndex, to.SlotIndex);
            else if (from.IsQuickSlot && !to.IsQuickSlot)
                InventoryManager.Instance.SwapItemBetweenBagAndQuickSlot(to.SlotIndex, from.SlotIndex);
            else
                InventoryManager.Instance.SwapItemWithinQuickSlot(from.SlotIndex, to.SlotIndex);
        }
        else if (from.IsStorageSlot || to.IsStorageSlot)
        {
            if (from.IsStorageSlot && !to.IsStorageSlot)
                InventoryManager.Instance.SwapItemBetweenStorageAndBag(from.SlotIndex, to.SlotIndex);
            else if (!from.IsStorageSlot && to.IsStorageSlot)
                InventoryManager.Instance.SwapItemBetweenStorageAndBag(to.SlotIndex, from.SlotIndex);
        }
        else
        {
            InventoryManager.Instance.SwapItemWithinBag(from.SlotIndex, to.SlotIndex);
        }
    }

    private void HandleRightClick()
    {
        if (IsStorageSlot || icon.sprite == null || IsQuickSlot)
        {
            // 만약 퀵슬롯에서 우클릭했을 때 '해제' 기능을 넣고 싶다면 여기에 작성
            if (IsQuickSlot)
            {
                Debug.Log("퀵슬롯 아이템은 가방에서 관리하거나 단축키를 이용하세요.");
            }
            return;
        }
        InventorySlot slotData = InventoryManager.Instance.BagSlots[SlotIndex];

        if (slotData != null && slotData.item != null)
        {
            if (slotData.item.itemType == ItemType.Useable)
            {
                Debug.Log("퀘스트 아이템은 조작할 수 없습니다.");
                return;
            }
            ItemMenuUI.Instance.ShowMenu(this, slotData);
        }

    }
}