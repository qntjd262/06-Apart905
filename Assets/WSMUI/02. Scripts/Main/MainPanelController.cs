using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainPanelController : MonoBehaviour
{
    // 프리팹과 인스턴스 변수들은 전부 UIManager로 이관되었으므로 삭제함.

    public void OnClickStartButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        UIManager.Instance.OpenSelectCharacterPanel();
    }

    public void OnClickLoadSaveButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        UIManager.Instance.OpenSaveLoadPanelAsLoadMode();
    }

    public void OnClickEndingListButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        UIManager.Instance.OpenEndingListPanel();
    }

    public void OnClickOptionButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        UIManager.Instance.OpenOptionPanel();
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