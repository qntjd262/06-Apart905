using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

            saveLoader.currentMode = Constants.ESaveLoadType.Save;
            saveLoader.gameObject.SetActive(true);
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

            saveLoader.currentMode = Constants.ESaveLoadType.Load;
            saveLoader.gameObject.SetActive(true);
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

            options.gameObject.SetActive(true);

            options.Open();
        }
    }

    public void OnMainMenuClick()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadScene(Constants.ESceneType.PrototypeMain);

            DOVirtual.DelayedCall(0.5f, () =>
            {
                HidePanel();
                UIManager.Instance.UnregisterUI(this.gameObject);
            }).SetUpdate(true);

            return;
        }

        Time.timeScale = 1f;
        HidePanel();
        SceneManager.LoadScene(Constants.ESceneType.PrototypeMain.ToString());
    }
}