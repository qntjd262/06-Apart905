using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스크립트")]
    [SerializeField] private PlayerMove playerMove;

    [Header("플레이어 상태")]
    public bool isDead =false;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if(isDead) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouch = Input.GetKey(KeyCode.LeftControl);

        playerMove.Move(h, v, isRunning, isCrouch);
    }
}
