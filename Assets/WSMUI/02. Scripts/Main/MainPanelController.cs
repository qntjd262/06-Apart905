using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainPanelController : MonoBehaviour
{
    public void OnClickStartButton()
    {
        // SoundManager.Instance.PlaySFX("Button_Click");
        UIManager.Instance.OpenSelectCharacterPanel();
        if (InventoryManager.Instance != null)
    {
        InventoryManager.Instance.ResetInventory();
    }
    // 퀘스트 진행도나 플레이어 스탯을 관리하는 싱글톤이 있다면 여기서 같이 초기화해야 한다.
    // QuestManager.Instance?.ResetQuests();
    // GameManager.Instance?.ResetGameData();
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