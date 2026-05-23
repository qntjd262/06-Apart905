using UnityEngine;
using UnityEngine.SceneManagement;

// 1. BasePopupUI 상속
public class PauseMenuController : BasePopupUI
{
    private void Awake()
    {
        popupPanel = this.gameObject;
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
        SaveLoadController saveLoader = FindFirstObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoader != null)
        {
            HidePanel();
            saveLoader.onCloseAction = () =>
            {
                this.ShowPanel();
            };
            saveLoader.Open(Constants.ESaveLoadType.Save);
        }
    }
    public void OnLoadClick()
    {
        SaveLoadController saveLoader = FindFirstObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoader != null)
        {
            HidePanel();
            saveLoader.onCloseAction = () =>
            {
                this.ShowPanel();
            };
            saveLoader.Open(Constants.ESaveLoadType.Load);
        }
    }

    public void OnOptionsClick()
    {
        OptionsController options = FindFirstObjectByType<OptionsController>(FindObjectsInactive.Include);
        if (options != null)
        {
            HidePanel();

            options.onCloseAction = () =>
            {
                this.ShowPanel();
            };

            // 3. 설정 창을 연다.
            options.Open();
        }
    }

    public void OnMainMenuClick()
    {
        if (UIManager.Instance != null)
        {
            // 씬 이동 시 일시정지 창을 스택에서 확실히 제거
            UIManager.Instance.TogglePauseMenu();
            UIManager.Instance.LoadScene(Constants.ESceneType.PrototypeMain);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(Constants.ESceneType.PrototypeMain.ToString());
    }
}