using UnityEngine;

public class EndingTrigger : MonoBehaviour, IInteractable
{
    [Header("Ending Data")]
    [SerializeField] private EndingData endingData;

    [Header("Trigger Settings")]
    [SerializeField] private bool isInteractionTrigger = true;
    [SerializeField] private bool isStepTrigger = false;

    [SerializeField] private EndingPopupUI endingPopupUI;

    public string GetInteractText()
    {
        return $"{endingData.endingName}"; 
    }

    public Constants.InteractType GetInteractType()
    {
        return Constants.InteractType.Door;
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

        if (endingPopupUI != null)
        {
            endingPopupUI.gameObject.SetActive(true); 
            endingPopupUI.ShowEndingPopup(endingData); 
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