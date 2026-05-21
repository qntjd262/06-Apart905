using UnityEngine;

public class EndingTrigger : MonoBehaviour, IInteractable
{
    [Header("Ending Data")]
    [SerializeField] private EndingData endingData;

    [Header("Trigger Settings")]
    [SerializeField] private bool isInteractionTrigger = true;
    [SerializeField] private bool isStepTrigger = false;

    // GameObject 대신 스크립트 컴포넌트를 직접 할당받습니다.
    [SerializeField] private EndingPopupUI endingPopupUI;

    public string GetInteractText()
    {
        return $"{endingData.endingName} 확인하기"; // '진행하기'보다 구체적으로 표기 가능
    }

    public Constants.InteractType GetInteractType()
    {
        return Constants.InteractType.Pickup;
    }

    public void Interact(PlayerStat player)
    {
        if (!isInteractionTrigger) return;

        TriggerEndingPopup();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isStepTrigger && other.CompareTag("Player"))
        {
            TriggerEndingPopup();
        }
    }

    private void TriggerEndingPopup()
    {
        if (endingData == null) return;

        // 1. 할당된 UI 오브젝트 자체가 꺼져 있을 수 있으므로 게임 오브젝트를 먼저 켭니다.
        if (endingPopupUI != null)
        {
            endingPopupUI.gameObject.SetActive(true); // 여기서 먼저 오브젝트를 강제로 활성화
            endingPopupUI.ShowEndingPopup(endingData); // 그 다음 데이터 할당 및 애니메이션 실행
        }
        else
        {
            if (endingPopupUI != null)
            {
                endingPopupUI.gameObject.SetActive(true);
                endingPopupUI.ShowEndingPopup(endingData);
            }
        }
    }
}