using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainPanelController : MonoBehaviour
{
    [Header("Panel Prefabs")]
    [SerializeField] private GameObject selectCharacterPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject endingListPanel;
    [SerializeField] private GameObject optionPanel;

    // 생성된 인스턴스 추적용 변수
    private GameObject _selectCharacterInstance;
    private GameObject _saveLoadInstance;
    private GameObject _endingListInstance;
    private GameObject _optionInstance;

    // 핵심: 중복되는 생성/활성화 로직을 캡슐화한 내부 메서드
    private void OpenPanel(ref GameObject instance, GameObject prefab)
    {
        if (instance != null)
        {
            instance.SetActive(true);
        }
        else
        {
            instance = Instantiate(prefab, UIManager.Instance.Canvas.transform);
        }
    }

    public void OnClickStartButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        OpenPanel(ref _selectCharacterInstance, selectCharacterPanel);
    }

    public void OnClickLoadSaveButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        OpenPanel(ref _saveLoadInstance, saveLoadPanel);
    }

    public void OnClickEndingListButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        OpenPanel(ref _endingListInstance, endingListPanel);
    }

    public void OnClickOptionButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        OpenPanel(ref _optionInstance, optionPanel);
    }

    public void OnClickExitButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}