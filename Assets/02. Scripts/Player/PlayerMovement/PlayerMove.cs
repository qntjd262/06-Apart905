using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;

    [Header("플레이어 속도")]

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("플레이어 속도 확인용")]
    [SerializeField] private float currentSpeed;

    [Header("중력 세팅")]
    private float gravity = -9f;
    private Vector3 velocity;
    
    [Header("스태미나 세팅")]
    private PlayerStat playerStat;
    [SerializeField] private float staminaDecreaseRate = 10f;
    [SerializeField] private float staminaRecoverRate = 0.05f;
    [SerializeField] private float recoveryDelay = 1f;
    public bool isExhausted = false;

    private bool isRecoveryPaused = false;


    [Header("플레이어 컴포넌트")]
    private Animator playerAnim;
    private PlayerNoise playerNoise;

    [Header("인벤토리 상태")]
    public bool isInventoryOpen = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerAnim = GetComponentInChildren<Animator>();
        playerStat = GetComponent<PlayerStat>();

        playerNoise = GetComponent<PlayerNoise>();
    }

    public void Move(float h, float v, bool isRunning, bool isCrouch)
    {   //npc와 대화 중 움직이지 않는 로직
        if(playerStat != null && playerStat.isInteracting || isInventoryOpen)
        {            
            PausePlayer();
            return;
        }


        if(controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 moveDir = (transform.right * h + transform.forward * v).normalized;
        
        bool isMoving = moveDir.magnitude > 0;

        if(isRunning && isMoving && !isExhausted && !isCrouch)
        {
            playerStat.stamina.DecreaseStat(staminaDecreaseRate * Time.deltaTime);

            if(playerStat.stamina.currentValue <= 0)
            {
                TriggerExhaustion();
            }
        }
        else
        {
            if(!isRecoveryPaused && playerStat.stamina.currentValue < playerStat.stamina.maxValue)
            {
                playerStat.stamina.AddStat(staminaRecoverRate * Time.deltaTime);
            }
        }

        //속도 및 소음 조절
        float noiseRadius = 0f;

        if (!isMoving)
        {
            currentSpeed = 0f;
            noiseRadius = 0f;
        }
        else if (isCrouch)
        {
            currentSpeed = crouchSpeed;
            noiseRadius = 3f;
        }
        else if(isRunning && !isExhausted)
        {
            currentSpeed = runSpeed;
            noiseRadius = 10f;
        }
        else
        {
            currentSpeed = moveSpeed;
            noiseRadius = 5f;
        }

        //소음 발생
        if(playerNoise != null)
        {
            playerNoise.SetNoiseRadius(noiseRadius);
        }
        

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(playerAnim != null)
        {
            float playerSpeed = moveDir.magnitude * currentSpeed;

            playerAnim.SetFloat("Speed", playerSpeed, 0.1f, Time.deltaTime);
            playerAnim.SetBool("IsCrouch", isCrouch);
        }


    }

    public void TriggerExhaustion()
    {
        if (!isExhausted)
        {
            StartCoroutine(ExhaustionRoutine());
        }
    }
    private IEnumerator ExhaustionRoutine()
    {
        isExhausted = true;

        isRecoveryPaused = true;

        yield return new WaitForSeconds(recoveryDelay);

        isRecoveryPaused = false;

        yield return new WaitUntil(() =>playerStat.stamina.currentValue >= playerStat.stamina.maxValue);

        isExhausted = false;
    }

    public void PausePlayer()
    {
        currentSpeed = 0f;

        if(playerAnim != null)
        {
            playerAnim.SetFloat("Speed",0f);
        }

        if(playerNoise != null)
        {
            playerNoise.SetNoiseRadius(0f);
        }  
    }

}
