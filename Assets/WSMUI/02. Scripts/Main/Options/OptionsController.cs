using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private GameObject[] tabPanels;
    [SerializeField] private Button[] tabButtons;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button returnButton;

    public bool ReturnToPauseMenuOnClose { get; set; }


    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmClick);
        returnButton.onClick.AddListener(OnReturnClick);
    }


    private void OnEnable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenPopupWithEffects("OPTIONS");
        }

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

            if (ReturnToPauseMenuOnClose)
            {
                ReturnToPauseMenuOnClose = false;
                UIManager.Instance.ShowPauseMenuWithoutChangingTimeScale();
            }
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
        PlayerPrefs.Save();
        Debug.Log("OptionsController: 설정 데이터가 저장되었습니다.");
    }

    private void OnReturnClick()
    {
        gameObject.SetActive(false);
    }
}
