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
        popupPanel = this.gameObject; // 부모 변수 연결

        confirmButton.onClick.AddListener(OnConfirmClick);
        returnButton.onClick.AddListener(OnReturnClick);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }
    }

    // 2. 이 Open() 함수가 있어야 PauseMenuController에서 호출할 수 있다.
    public void Open()
    {
        ShowPanel(); // UIManager 스택 등록 및 창 켜기
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
        if (soundOptions != null) soundOptions.SaveOptions();
        PlayerPrefs.Save();
        Debug.Log("OptionsController: 설정 데이터가 저장되었습니다.");
    }

    private void OnReturnClick()
    {
        if (soundOptions != null) soundOptions.RevertOptions();
        HidePanel();
    }
}