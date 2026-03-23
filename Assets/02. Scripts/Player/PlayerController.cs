using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스크립트")]
    private PlayerMove playerMove;
    private PlayerLook playerLook;

    [Header("플레이어 상태")]
    public bool isDead =false;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerLook = GetComponent<PlayerLook>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(isDead) return;

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
    }
}
