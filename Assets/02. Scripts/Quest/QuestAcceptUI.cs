using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class QuestAcceptUI : MonoBehaviour
{
    public static QuestAcceptUI Instance { get; private set; }

    [Header("UI 콤포넌트")]
    public GameObject popupPanel;       
    public TextMeshProUGUI titleText;    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (popupPanel != null) 
            popupPanel.SetActive(false);
    }

    public void ShowPopup(Quest quest)
    {
        if (popupPanel == null) return;

        if (titleText != null)
        {
            titleText.text = $"[{quest.questName}]\n퀘스트를 수락하시겠습니까?";
        }

        popupPanel.SetActive(true);
    }

    public void OnClickYes()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ConfirmAcceptQuest();
        }
        popupPanel.SetActive(false); 
    }

    public void OnClickNo()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CancelQuest(); 
        }
        popupPanel.SetActive(false); 
    }
}
