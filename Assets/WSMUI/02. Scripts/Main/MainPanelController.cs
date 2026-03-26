#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private GameObject selectCharacterPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject endingListPanel;
    [SerializeField] private GameObject optionPanel;

    private GameObject _selectCharacterInstance;
    private GameObject _optionInstance;
    private GameObject _saveLoadInstance;
    private GameObject _endingListInstance;

    public void OnClickStartButton()
    {
        //SoundManager.Instance.PlaySFX("Button_Click");
        if (_selectCharacterInstance)
        {
            _selectCharacterInstance.SetActive(true);
        }
        else
        {
            _selectCharacterInstance = Instantiate(selectCharacterPanel, UIManager.Instance.Canvas.transform);
        }
    }

    public void OnClickLoadSaveButton()
    {
        //SoundManager.Instance.PlaySFX("Button_Click");

        if (_saveLoadInstance)
        {
            _saveLoadInstance.SetActive(true);
        }
        else
        {
            _saveLoadInstance = Instantiate(saveLoadPanel, UIManager.Instance.Canvas.transform);
        }

    }

    public void OnClickEndingListButton()
    {
        //SoundManager.Instance.PlaySFX("Button_Click");

        if (_endingListInstance)
        {
            _endingListInstance.SetActive(true);
        }
        else
        {
            _endingListInstance = Instantiate(endingListPanel, UIManager.Instance.Canvas.transform);
        }
    }

    public void OnClickOptionButton()
    {
        //SoundManager.Instance.PlaySFX("Button_Click");

        if (_optionInstance)
        {
            _optionInstance.SetActive(true);
        }
        else
        {
            _optionInstance = Instantiate(optionPanel, UIManager.Instance.Canvas.transform);
        }
    }

    public void OnClickExitButton()
    {
        //SoundManager.Instance.PlaySFX("Button_Click");

        Application.Quit();


#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}
