using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestNotifyUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private Button acceptButton;

    [Header("Color Settings")]
    [SerializeField] private Color mainQuestColor = new Color(0.8f, 0.4f, 0f);
    [SerializeField] private Color subQuestColor = Color.blue;

    private Quest currentQuest;

    private void Awake()
    {
        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        popupPanel.transform.localScale = Vector3.zero;
        popupPanel.SetActive(false);
    }

    public void ShowNotice(Quest quest)
    {
        currentQuest = quest;

        // 1. 데이터 세팅
        titleText.text = quest.questName;
        titleText.color = quest.isMainQuest ? mainQuestColor : subQuestColor;
        
        // 스토리와 목표 세팅 (기존 Quest 데이터 구조 활용)
        storyText.text = quest.beforeAcceptDialogues.Length > 0 ? quest.beforeAcceptDialogues[0] : "새로운 임무가 부여되었습니다.";
        goalText.text = $"{quest.targetID} ({quest.currentAmount}/{quest.goalAmount})";

        // 2. 팝업 연출 (중앙에서 커지며 등장)
        popupPanel.SetActive(true);
        popupPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        
        // 커서 상태 갱신은 UIManager가 담당
        UIManager.Instance.UpdateCursorState();
    }

    private void OnAcceptButtonClicked()
    {
        // 팝업 닫기 연출
        popupPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            popupPanel.SetActive(false);
            
            // 퀘스트 실제 수락 처리
            QuestManager.Instance.AcceptQuest(currentQuest);
            
            // 커서 상태 다시 갱신
            UIManager.Instance.UpdateCursorState();
        });
    }
}