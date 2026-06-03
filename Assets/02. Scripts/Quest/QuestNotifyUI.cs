using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestNotifyUI : BasePopupUI
{
    public static QuestNotifyUI Instance { get; private set;}

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private Button acceptButton;

    [Header("Color Settings")]
    [SerializeField] private Color mainQuestColor = new Color(0.8f, 0.4f, 0f);
    [SerializeField] private Color subQuestColor = Color.green;

    public bool IsProcessing { get; private set; }

    private Quest currentQuest;
    
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        
        if (popupPanel != null)
        {
            popupPanel.transform.localScale = Vector3.zero;
            popupPanel.SetActive(false); // 씬 시작 시 단순 비활성화 (스택 로직 타지 않음)
        }
    }

    public void ShowNotice(Quest quest)
    {
        currentQuest = quest;

        // 1. 데이터 세팅
        titleText.text = quest.questName;
        titleText.color = quest.isMainQuest ? mainQuestColor : subQuestColor;
        
        // 스토리와 목표 세팅 (기존 Quest 데이터 구조 활용)
        storyText.text = quest.questDiscrip;
        //storyText.text = quest.beforeAcceptDialogues.Length > 0 ? quest.beforeAcceptDialogues[0] : "새로운 임무가 부여되었습니다.";
        goalText.text = $"□ {quest.questGoal}";
        //goalText.text = $"□ {quest.targetID} ({quest.currentAmount}/{quest.goalAmount})"; 수정 전 대목

        ShowPanel();

        // 3. 자식만의 고유 연출 (중앙에서 커지며 등장)
        popupPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void OnAcceptButtonClicked()
    {
        // 버튼 연타 방지
        acceptButton.interactable = false;
        
        IsProcessing = true;

        // 팝업 닫기 연출
        popupPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            HidePanel();
            
            if(QuestManager.Instance != null)
            {
                QuestManager.Instance.ConfirmAcceptQuest();
            }
            
            acceptButton.interactable = true;

            StartCoroutine(ReleasePlayerNextFrame());
        });
    }

    private System.Collections.IEnumerator ReleasePlayerNextFrame()
    {
        yield return null;
        IsProcessing = false; // 이제 공격 가능하도록 해제
    }
}