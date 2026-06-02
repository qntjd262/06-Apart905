using UnityEngine;
using UnityEngine.UI;

// 1. MonoBehaviour가 아니라 BasePopupUI를 상속받아야 한다.
public class OptionsController : BasePopupUI
{
    [SerializeField] private GameObject[] tabPanels;
    [SerializeField] private Button[] tabButtons;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button returnButton;

    [Header("세부 옵션 컴포넌트 연결")]
    [SerializeField] private SoundOptions soundOptions;
    [SerializeField] private GraphicOptions graphicOptions;
    [SerializeField] private KeyBindOptions keyBindOptions;

    private void Awake()
    {
        popupPanel = this.gameObject;

        confirmButton.onClick.AddListener(OnConfirmClick);
        returnButton.onClick.AddListener(OnReturnClick);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }
    }

    public void Open()
    {
        ShowPanel();
    }

    private void OnEnable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenPopupWithEffects("설정");
        }

        if (soundOptions != null) soundOptions.Initialize();
        if (graphicOptions != null) graphicOptions.Initialize();
        if (keyBindOptions != null) keyBindOptions.Initialize();

        if (tabPanels != null && tabPanels.Length > 0)
        {
            SwitchTab(0);
        }
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClosePopupWithEffects();
        }
    }

    private void SwitchTab(int tabIndex)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            if (i == tabIndex)
            {
                tabPanels[i].SetActive(true);
                tabButtons[i].interactable = false;
            }
            else
            {
                tabPanels[i].SetActive(false);
                tabButtons[i].interactable = true;
            }
        }
    }

    private void OnConfirmClick()
    {
        // 모든 옵션 컴포넌트들의 실제 물리 저장 프로세스 일괄 가동
        if (soundOptions != null) soundOptions.SaveOptions();
        if (graphicOptions != null) graphicOptions.SaveOptions();
        if (keyBindOptions != null) keyBindOptions.SaveOptions(); // 추가

        PlayerPrefs.Save();
        Debug.Log("OptionsController: 설정 데이터가 저장되었습니다.");
    }

    private void OnReturnClick()
    {
        // 디스크 저장 없이 창을 닫을 경우, 임시 변경 값들을 전부 원본 상태로 롤백
        if (soundOptions != null) soundOptions.RevertOptions();
        if (graphicOptions != null) graphicOptions.RevertOptions();
        if (keyBindOptions != null) keyBindOptions.RevertOptions(); // 추가

        HidePanel();
    }
}