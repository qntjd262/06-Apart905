using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("시점 회전 속도")]
    [SerializeField] private float mouseSensitivity = 2f;


    [Header("카메라 세팅")]
    [SerializeField] private Transform cameraPos;
    //상하 제한
    [SerializeField] private float upDownRange = 80f;
    //현재 카메라 상하각도 저장 변수
    private float currentVerticalRotation = 0f;
    private float standHeight;
    private float crouchHeight = 2.3f;
    private float cameraForwardOffset = 0.35f;

    [Header("카메라 벽뚫림 방지")]
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float cameraRadius = 0.12f;

    private PlayerStat playerStat;
    private Camera mainCam;

    private bool isCrouching = false;


    void Start()
    {
        playerStat = GetComponent<PlayerStat>();

        mainCam = Camera.main;

        if(mainCam != null && cameraPos != null)
        {
            //카메라 cameraPos 오브젝트의 자식 이동
            mainCam.transform.SetParent(cameraPos);

            //카메라 위치, 회전값 cameraPos 오브젝트와 동일하게 맞추기
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.identity;

            standHeight = cameraPos.localPosition.y;
        }
    }
    public void Look(float mouseX, float mouseY)
    {
        if(playerStat.isInteracting) return;

        transform.Rotate(0f, mouseX * mouseSensitivity, 0f);
        
        currentVerticalRotation -= mouseY * mouseSensitivity;

        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, -upDownRange, upDownRange);

        if(cameraPos != null)
        {
            cameraPos.localEulerAngles = new Vector3(currentVerticalRotation, 0f, 0f);
        }
    }

    public void SetCameraHeight(bool isCrouch)
    {
        isCrouching = isCrouch;
        if(cameraPos != null)
        {
            Vector3 newPos = cameraPos.localPosition;
            newPos.y = isCrouch ? crouchHeight : standHeight;

            cameraPos.localPosition = newPos;
        }
    }

    private void LateUpdate()
    {
        if(cameraPos == null) return;

        float targetZOffset = isCrouching ? cameraForwardOffset : 0f;

        Vector3 originPos = transform.position + transform.up * cameraPos.localPosition.y;

        float finalZOffset = targetZOffset;

        if(targetZOffset > 0f)
        {
            RaycastHit hit;

            if(Physics.SphereCast(originPos, cameraRadius, transform.forward, out hit, targetZOffset, blockLayer))
            {
                finalZOffset = Mathf.Max(0f, hit.distance);
            }
        }

        Vector3 currentLocalPos = cameraPos.localPosition;
        currentLocalPos.z = finalZOffset;
        cameraPos.localPosition = currentLocalPos;

        if(mainCam != null)
        {
            mainCam.transform.localPosition = Vector3.zero;
        }
                
    }
}
