using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f; //상호작용 거리
    public LayerMask interactLayer;     //아이템의 Layer

    public LayerMask obstacleLayer; //플레이어 시야를 가리는 장애물 레이어

    private Camera cam;
    private PlayerStat playerStat;

    void Start()
    {
        cam = Camera.main;
        playerStat = GetComponent<PlayerStat>();
    }

    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        int combinedLayerMask = interactLayer | obstacleLayer;

        if (Physics.Raycast(ray, out hit, interactDistance, combinedLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                UIManager.Instance.ShowInteractUI(interactable.GetInteractText(), interactable.GetInteractType());

                if (InputManager.Instance.GetKeyDown(EKeyAction.Interact))
                {
                    Debug.Log($"[Interact] 키 입력 감지! {hit.collider.gameObject.name}와 상호작용을 시도합니다.");
                    interactable.Interact(playerStat);
                }
                return;
            }
        }
        UIManager.Instance.HideInteractUI();
    }
}
