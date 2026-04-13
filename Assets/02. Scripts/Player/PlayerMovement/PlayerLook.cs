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

    private PlayerStat playerStat;


    void Start()
    {
        playerStat = GetComponent<PlayerStat>();

        Camera mainCam = Camera.main;

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
        if(cameraPos != null)
        {
            Vector3 newPos = cameraPos.localPosition;
            newPos.y = isCrouch ? crouchHeight : standHeight;
            newPos.z = isCrouch ? cameraForwardOffset : 0f;
            cameraPos.localPosition = newPos;
        }
    }
}
