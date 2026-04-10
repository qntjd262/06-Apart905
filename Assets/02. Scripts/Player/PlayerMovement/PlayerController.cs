using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스크립트")]
    private PlayerMove playerMove;
    private PlayerLook playerLook;
    private PlayerAttack playerAttack;

    [Header("플레이어 상태")]
    public bool isDead =false;

    //[Header("인벤토리 세팅")]
    

    

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerLook = GetComponent<PlayerLook>();
        playerAttack = GetComponent<PlayerAttack>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(isDead) return;
        
        /* UIManager 인벤토리 관리 TODO : ToggleInventory() -> public , isInventoryOpen 프로퍼티 추가
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab) || (UIManager.Instance.isInventoryOpen && Input.GetKeyDown(KeyCode.Escape)))
        {
            UIManager.Instance.ToggleInventory();
        }

        if(UIManager.Instance.isInventoryOpen) return;
        */
            
        

        //플레이어 회전 + 카메라
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        playerLook.Look(mouseX, mouseY);

        //플레이어 움직임

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouch = Input.GetKey(KeyCode.LeftControl);


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            playerLook.SetCameraHeight(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            playerLook.SetCameraHeight(false);
        }

        playerMove.Move(h, v, isRunning, isCrouch);

        //플레이어 공격
        if (Input.GetMouseButtonDown(0))
        {   
            //TODO : 공격 애니메이션을 통해 해당 애니메이션 지점에서 Attack()함수 실행하기
            playerAttack.Attack();
        }

    }
}
