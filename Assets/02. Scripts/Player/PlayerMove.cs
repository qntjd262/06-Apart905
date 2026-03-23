using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;

    [Header("플레이어 속도")]

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float RunSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("플레이어 속도 확인용")]
    [SerializeField] private float currentSpeed;

    [Header("중력 세팅")]
    private float gravity = -9f;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Move(float h, float v, bool isRunning, bool isCrouch)
    {
        if(controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 moveDir = (transform.right * h + transform.forward * v).normalized;

        currentSpeed = moveSpeed;

        if (isCrouch)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isRunning)
        {
            currentSpeed = RunSpeed;
        }

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


    }





}
