using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public SaveLoadController saveLoadController;

    private void Awake()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.pauseMenuPanel = this.gameObject;
        }
    }

    private void OnEnable()
    {
        if (UIManager.Instance != null) UIManager.Instance.OpenPopupWithEffects("일시정지");
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null) UIManager.Instance.ClosePopupWithEffects();
    }

    public void OnContinueClick()
    {
        UIManager.Instance.TogglePauseMenu();
    }


    public void OnSaveClick()
    {
        // 하이어라키에 꺼져있는 SaveLoad 패널을 찾아서 모드를 'Save'로 주입하고 켠다
        SaveLoadController saveLoader = FindFirstObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoader != null)
        {
            saveLoader.currentMode = Constants.ESaveLoadType.Save;
            saveLoader.ReturnToPauseMenuOnClose = true;
            gameObject.SetActive(false);
            saveLoader.gameObject.SetActive(true);
        }
    }

    public void OnLoadClick()
    {
        // 똑같은 패널을 찾지만, 이번에는 모드를 'Load'로 주입하고 켠다
        SaveLoadController saveLoader = FindFirstObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoader != null)
        {
            saveLoader.currentMode = Constants.ESaveLoadType.Load;
            saveLoader.ReturnToPauseMenuOnClose = true;
            gameObject.SetActive(false);
            saveLoader.gameObject.SetActive(true);
        }
    }
    public void OnOptionsClick()
    {
        OptionsController options = FindFirstObjectByType<OptionsController>(FindObjectsInactive.Include);
        if (options != null)
        {
            options.ReturnToPauseMenuOnClose = true;
            gameObject.SetActive(false);
            options.gameObject.SetActive(true);
        }
    }

    public void OnMainMenuClick()
    {
        // SetActive(false) 전에 LoadScene 먼저 → OnDisable 타이밍 문제 제거
        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadScene(Constants.ESceneType.TestMainScene);
            gameObject.SetActive(false); // LoadScene 이후에 끄기
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(Constants.ESceneType.TestMainScene.ToString());
    }
}
