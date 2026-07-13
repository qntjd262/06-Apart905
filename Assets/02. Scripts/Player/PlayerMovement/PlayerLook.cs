using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("시점 회전 속도")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("카메라 세팅")]
    [SerializeField] private Transform cameraPos;     
    [SerializeField] private Transform cameraSocket;  

    // 상하 제한
    [SerializeField] private float maxLookUp = -80f;
    [SerializeField] private float maxLookDown = 50f;
    // 현재 카메라 상하각도 저장 변수
    private float currentVerticalRotation = 0f;

    [Header("카메라 벽뚫림 방지")]
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float cameraRadius = 0.12f;

    [Header("쪼그려 앉아있을때 카메라 세팅")]
    [SerializeField] private float crouchZOffset = 0.2f;

    private PlayerStat playerStat;
    private PlayerController playerController;
    private Camera mainCam;

    void Start()
    {
        playerStat = GetComponent<PlayerStat>();
        playerController = GetComponent<PlayerController>();
        mainCam = Camera.main;

        if (mainCam != null && cameraPos != null)
        {
            mainCam.transform.SetParent(cameraPos);
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.identity;
        }
    }

    public void Look(float mouseX, float mouseY)
    {
        if (playerStat.isInteracting || playerController.isDead) return;

        // 좌우 회전: 플레이어 몸체 회전
        float sensH = InputManager.Instance.MouseSensH;
        float senV = InputManager.Instance.MouseSensV;

        transform.Rotate(0f, mouseX * sensH, 0f);
        
        currentVerticalRotation -= mouseY * senV;
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, maxLookUp, maxLookDown);

        // transform.Rotate(0f, mouseX * mouseSensitivity, 0f);

        // currentVerticalRotation -= mouseY * mouseSensitivity;

        if (cameraPos != null)
        {
            cameraPos.localEulerAngles = new Vector3(currentVerticalRotation, 0f, 0f);
        }
    }

    private void LateUpdate()
    {
        if (cameraPos == null || cameraSocket == null) return;

        cameraPos.position = cameraSocket.position;

        if(playerController.isDead)
        {
            cameraPos.rotation = cameraSocket.rotation;
        }

        float targetZOffset = playerController.isCrouch ? crouchZOffset : 0f;
        float finalZOffset = 0f;
        RaycastHit hit;

        float castDistance = Mathf.Abs(targetZOffset) + 0.1f;

        if (Physics.SphereCast(cameraSocket.position - (cameraPos.forward * 0.2f), cameraRadius, cameraPos.forward, out hit, castDistance, blockLayer))
        {
            finalZOffset = -(hit.distance - cameraRadius);

            finalZOffset = Mathf.Min(finalZOffset, 0f);
        }
        else
        {
            if (Physics.SphereCast(cameraSocket.position - (cameraPos.forward * 0.2f), cameraRadius, cameraPos.forward, out hit, 0.2f, blockLayer))
            {
                finalZOffset = -0.1f;
            }
        }

        if (mainCam != null)
        {
            Vector3 camLocalPos = mainCam.transform.localPosition;
            camLocalPos.z = finalZOffset;
            mainCam.transform.localPosition = camLocalPos;
        }
    }

    /* 해당 부분 카메라가 플레이어 Head 오브젝트의 자식으로 부착되어있기 때문에 필요 X
    public void SetCameraHeight(bool isCrouch)
    {
        if(cameraPos != null)
        {
            Vector3 newPos = cameraPos.localPosition;
            newPos.y = isCrouch ? 0.8f : 1.6f;
            newPos.z = isCrouch ? cameraForwardOffset : 0f;

        }
    }
    */
}
