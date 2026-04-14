using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    private void OnEnable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OpenPopupWithEffects("GAME OVER");
        
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ClosePopupWithEffects();
    }

    public void OnLoadClick()
    {
        Time.timeScale = 1f;
        SaveLoadController saveLoader = FindFirstObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoader != null)
        {
            saveLoader.currentMode = Constants.ESaveLoadType.Load;
            gameObject.SetActive(false);
            saveLoader.gameObject.SetActive(true);
        }
    }

    public void OnMainMenuClick()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.LoadScene(Constants.ESceneType.Main);

        Time.timeScale = 1f;
    }
}