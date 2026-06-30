using UnityEngine;
using UnityEngine.UI;

// 1. MonoBehaviour가 아니라 BasePopupUI를 상속받아야 한다.
public class OptionsController : BasePopupUI
{
    [SerializeField] private GameObject[] tabPanels;
    [SerializeField] private Button[] tabButtons;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button resetButton;

    [Header("세부 옵션 컴포넌트 연결")]
    //[SerializeField] private GeneralOptions generalOptions;
    [SerializeField] private SoundOptions soundOptions;
    [SerializeField] private GraphicOptions graphicOptions;
    [SerializeField] private ControlOptions controlOptions;

    private int currentTabIndex = 0;

    private void Awake()
    {
        popupPanel = this.gameObject;

        confirmButton.onClick.AddListener(OnConfirmClick);
        returnButton.onClick.AddListener(OnReturnClick);

        if (resetButton != null) resetButton.onClick.AddListener(OnResetClick);

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
        if (controlOptions != null) controlOptions.Initialize();
        // if (generalOptions != null) generalOptions.Initialize();

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
        currentTabIndex = tabIndex; // [추가] 현재 선택된 탭 번호 기억하기

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

    private void OnResetClick()
    {
        switch (currentTabIndex)
        {
            // case 0:
            //     if (generalOptions != null) generalOptions.ResetToDefault();
            //     break;
            case 0:
                if (graphicOptions != null) graphicOptions.ResetToDefault();
                break;
            case 1:
                if (soundOptions != null) soundOptions.ResetToDefault();
                break;
            case 2:
                if (controlOptions != null) controlOptions.ResetToDefault();
                break;
        }
    }

    private void OnConfirmClick()
    {
        // 모든 옵션 컴포넌트들의 실제 물리 저장 프로세스 일괄 가동
        if (soundOptions != null) soundOptions.SaveOptions();
        if (graphicOptions != null) graphicOptions.SaveOptions();
        if (controlOptions != null) controlOptions.SaveOptions(); // 추가
        //if (generalOptions != null) generalOptions.SaveOptions(); // 추가

        PlayerPrefs.Save();
        Debug.Log("OptionsController: 설정 데이터가 저장되었습니다.");
    }

    public void OnReturnClick()
    {
        if (soundOptions != null) soundOptions.RevertOptions();
        if (graphicOptions != null) graphicOptions.RevertOptions();
        if (controlOptions != null) controlOptions.RevertOptions(); // 추가
       // if (generalOptions != null) generalOptions.RevertOptions(); // 추가

        HidePanel();
    }
}