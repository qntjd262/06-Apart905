using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스크립트")]
    private PlayerMove playerMove;
    private PlayerLook playerLook;
    private PlayerAttack playerAttack;
    private PlayerEquip playerEquip;


    [Header("플레이어 상태")]
    public bool isDead = false;

    //[Header("인벤토리 세팅")]




    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerLook = GetComponent<PlayerLook>();
        playerAttack = GetComponent<PlayerAttack>();
        playerEquip = GetComponent<PlayerEquip>();
    }
    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Update()
    {
        if (isDead) return;

        // if (InputManager.Instance.GetKeyDown(EKeyAction.Inventory) || Input.GetKeyDown(KeyCode.Tab))
        // {
        //     UIManager.Instance.ToggleInventory();
        // }
        bool isUIOpen = (UIManager.Instance.inventoryPanel != null && UIManager.Instance.inventoryPanel.activeSelf)
                            || UIManager.Instance.IsAnyPopupOpen;

        if (isUIOpen) return;
        // bool isInventoryOpen = UIManager.Instance.inventoryPanel.activeSelf;

        // if (isInventoryOpen)
        // {
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;

        //     return;
        // }
        // else
        // {
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }



        //플레이어 회전 + 카메라
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        playerLook.Look(mouseX, mouseY);

        //플레이어 움직임

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouch = Input.GetKey(KeyCode.LeftControl);


        playerMove.Move(h, v, isRunning, isCrouch);

        HandleQuickSlotItemEquip();

        //플레이어 공격
        if (Input.GetMouseButtonDown(0))
        {
            //TODO : 공격 애니메이션을 통해 해당 애니메이션 지점에서 Attack()함수 실행하기
            playerEquip.UseCurrentItem();
        }

    }

    private void HandleQuickSlotItemEquip()
    {
        if(playerEquip == null) return;

        if(Input.GetKeyDown(KeyCode.Alpha1)) playerEquip.EquipFromQuickSlot(0);
        else if(Input.GetKeyDown(KeyCode.Alpha2)) playerEquip.EquipFromQuickSlot(1);
        else if(Input.GetKeyDown(KeyCode.Alpha3)) playerEquip.EquipFromQuickSlot(2);
        else if(Input.GetKeyDown(KeyCode.Alpha4)) playerEquip.EquipFromQuickSlot(3);
        else if(Input.GetKeyDown(KeyCode.Alpha5)) playerEquip.EquipFromQuickSlot(4);
    }
    
}
