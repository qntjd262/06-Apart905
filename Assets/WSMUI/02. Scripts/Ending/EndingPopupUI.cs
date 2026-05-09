using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class EndingPopupUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    [Header("Settings")]
    private Color mainQuestColor = new Color(0.8f, 0.4f, 0f); // 어두운 주황색
    private EndingData currentEnding;

    private void Awake()
    {
        acceptButton.onClick.AddListener(OnAccept);
        cancelButton.onClick.AddListener(OnCancel);
        popupPanel.SetActive(false);
    }

    public void ShowEndingPopup(EndingData data)
    {
        currentEnding = data;

        // 리치 텍스트를 사용하여 엔딩 이름만 색상을 바꿉니다.
        string coloredEndingName = $"<color=#{ColorUtility.ToHtmlStringRGB(mainQuestColor)}>{data.endingName}</color>";
        noticeText.text = $"{coloredEndingName} 엔딩\n진행하시겠습니까?";

        popupPanel.SetActive(true);
        popupPanel.transform.localScale = Vector3.zero;
        popupPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        UIManager.Instance.UpdateCursorState();
    }

    private void OnAccept()
    {
        // 버튼 중복 클릭 방지
        acceptButton.interactable = false;

        GameManager.Instance.selectedEnding = currentEnding;
        // 1. 팝업을 살짝 닫아주며 연출 시작
        popupPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {

            // 2. UIManager의 FadeOut만 사용 (로딩 패널 X)
            UIManager.Instance.FadeOut(1.5f, () =>
            {
                // 3. 페이드가 완전히 검어지면 씬 전환
                SceneManager.LoadScene("PrototypeEnding");
            });
        });
    }

    private void OnCancel()
    {
        popupPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {
            popupPanel.SetActive(false);
            UIManager.Instance.UpdateCursorState();
        });
    }
}