using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("시점 회전 속도")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("카메라 세팅")]
    [SerializeField] private Transform cameraPos;     
    [SerializeField] private Transform cameraSocket;  

    // 상하 제한
    [SerializeField] private float upDownRange = 80f;
    // 현재 카메라 상하각도 저장 변수
    private float currentVerticalRotation = 0f;

    [Header("카메라 벽뚫림 방지")]
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float cameraRadius = 0.12f;

    private PlayerStat playerStat;
    private Camera mainCam;

    void Start()
    {
        playerStat = GetComponent<PlayerStat>();
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
        if (playerStat.isInteracting) return;

        // 좌우 회전: 플레이어 몸체 회전
        transform.Rotate(0f, mouseX * mouseSensitivity, 0f);

        // 상하 회전: cameraPos 회전
        currentVerticalRotation -= mouseY * mouseSensitivity;
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, -upDownRange, upDownRange);

        if (cameraPos != null)
        {
            cameraPos.localEulerAngles = new Vector3(currentVerticalRotation, 0f, 0f);
        }
    }

    private void LateUpdate()
    {
        if (cameraPos == null || cameraSocket == null) return;

        cameraPos.position = cameraSocket.position;

        float finalZOffset = 0f;
        RaycastHit hit;

        if (Physics.SphereCast(cameraSocket.position - (cameraPos.forward * 0.2f), cameraRadius, cameraPos.forward, out hit, 0.2f, blockLayer))
        {
            finalZOffset = -0.1f;
        }

        if (mainCam != null)
        {
            Vector3 camLocalPos = mainCam.transform.localPosition;
            camLocalPos.z = finalZOffset;
            mainCam.transform.localPosition = camLocalPos;
        }
    }
}
