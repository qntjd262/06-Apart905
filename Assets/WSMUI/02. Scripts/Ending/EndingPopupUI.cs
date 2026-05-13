using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

// MonoBehaviour 대신 BasePopupUI 상속
public class EndingPopupUI : BasePopupUI 
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    [Header("Settings")]
    private Color mainQuestColor = new Color(0.8f, 0.4f, 0f);
    private EndingData currentEnding;

    private void Awake()
    {
        acceptButton.onClick.AddListener(OnAccept);
        cancelButton.onClick.AddListener(OnCancel);
        
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void ShowEndingPopup(EndingData data)
    {
        currentEnding = data;

        string coloredEndingName = $"<color=#{ColorUtility.ToHtmlStringRGB(mainQuestColor)}>{data.endingName}</color>";
        noticeText.text = $"{coloredEndingName} 엔딩\n진행하시겠습니까?";

        // 부모의 공통 함수 호출 (SetActive(true) 및 스택 등록 자동 수행)
        ShowPanel();

        // 자식만의 고유 연출
        popupPanel.transform.localScale = Vector3.zero;
        popupPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void OnAccept()
    {
        acceptButton.interactable = false;
        GameManager.Instance.selectedEnding = currentEnding;
        
        popupPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {
            // 부모의 공통 함수 호출 (스택 해제 및 SetActive(false) 자동 수행)
            HidePanel(); 
            
            UIManager.Instance.FadeOut(1.5f, () =>
            {
                SceneManager.LoadScene("PrototypeEnding");
            });
        });
    }

    private void OnCancel()
    {
        popupPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {
            // 연출이 끝난 후 부모 함수를 통해 안전하게 해제 및 종료
            HidePanel();
        });
    }
}