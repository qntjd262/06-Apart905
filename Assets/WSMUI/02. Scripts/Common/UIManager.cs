using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Canvases")]
    [SerializeField] private Canvas globalCanvas;
    [SerializeField] private Canvas hudCanvas;

    public Canvas CurrentCanvas => hudCanvas != null ? hudCanvas : globalCanvas;

    [Header("Global Effects")]
    [SerializeField] private GameObject dimBackground;
    [SerializeField] private GameObject cinematicBars;
    [SerializeField] private TextMeshProUGUI globalWindowTitleText;

    [Header("UI Panels (Local)")]
    [SerializeField] public GameObject inventoryPanel;
    public GameObject pauseMenuPanel;

    [Header("Global Popup Panels")]
    [SerializeField] private GameObject selectCharacterPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject endingListPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private LoadingPanelController loadingPanelPrefab;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private HUDController _hudController;
    private int activePopupCount = 0;

    public bool IsAnyPopupOpen => activePopupCount > 0;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this && globalCanvas == null)
        {
            globalCanvas = GetComponentInChildren<Canvas>();
        }

    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += InitializeInGameUI;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != Constants.ESceneType.Game.ToString()) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsAnyGlobalPopupActive()) return;

            if (inventoryPanel != null && inventoryPanel.activeSelf)
            {
                ToggleInventory();
            }
            else if (pauseMenuPanel != null)
            {
                TogglePauseMenu();
            }
        }

        //playercontroller에서 인벤토리 토글 담당
        /*
        bool isPaused = pauseMenuPanel != null && pauseMenuPanel.activeSelf;
        bool isBlockedByPopup = IsAnyGlobalPopupActive();
        if (!isPaused && !isBlockedByPopup && (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab)))
        {
            if (inventoryPanel != null) ToggleInventory();
        }
        */
    }

    public void OpenSelectCharacterPanel()
    {
        if (selectCharacterPanel != null)
        {
            selectCharacterPanel.SetActive(true);
        }
    }

    public void CloseSelectCharacterPanel()
    {
        if (selectCharacterPanel != null)
            selectCharacterPanel.SetActive(false);
    }

    public void OpenSaveLoadPanelAsLoadMode()
    {
        if (saveLoadPanel != null)
        {
            SaveLoadController controller = saveLoadPanel.GetComponent<SaveLoadController>();
            if (controller != null)
            {
                controller.currentMode = Constants.ESaveLoadType.Load;
            }

            saveLoadPanel.SetActive(true);
        }
    }

    public void OpenEndingListPanel()
    {
        if (endingListPanel != null)
        {
            endingListPanel.SetActive(true);
        }
    }

    public void OpenOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
        }
    }

    public void OpenPopupWithEffects(string title)
    {
        activePopupCount++;

        if (activePopupCount == 1)
        {
            if (dimBackground != null) dimBackground.SetActive(true);
            if (cinematicBars != null) cinematicBars.SetActive(true);
        }

        if (globalWindowTitleText != null && !string.IsNullOrEmpty(title))
        {
            globalWindowTitleText.text = title;
        }
    }

    public void ClosePopupWithEffects()
    {
        activePopupCount--;

        if (activePopupCount <= 0)
        {
            activePopupCount = 0;

            if (dimBackground != null) dimBackground.SetActive(false);
            if (cinematicBars != null) cinematicBars.SetActive(false);
            if (globalWindowTitleText != null) globalWindowTitleText.text = string.Empty;
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuPanel == null) return;

        bool isPaused = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ShowPauseMenuWithoutChangingTimeScale()
    {
        if (pauseMenuPanel != null && !pauseMenuPanel.activeSelf)
        {
            pauseMenuPanel.SetActive(true);
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("UIManager: inventoryPanel reference is missing.");
            return;
        }

        bool isNowActive = !inventoryPanel.activeSelf;

        inventoryPanel.SetActive(isNowActive);

        if (_hudController == null)
        {
            _hudController = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
        }

        if (_hudController != null)
        {
            _hudController.gameObject.SetActive(!isNowActive);
        }
    }

    private void InitializeInGameUI()
    {
        if (_hudController == null)
        {
            _hudController = FindFirstObjectByType<HUDController>();
        }

        if (_hudController != null)
        {
            _hudController.InitHUD();
        }
    }

    public void LoadScene(Constants.ESceneType sceneType, bool withLoadingPanel = true)
    {
        StartCoroutine(LoadSceneAsync(sceneType, withLoadingPanel));
    }

    private IEnumerator LoadSceneAsync(Constants.ESceneType sceneType, bool withLoadingPanel = true)
    {
        Time.timeScale = 1f;

        // 1. 화면 검게
        bool fadeDone = false;
        FadeOut(0.5f, () => fadeDone = true);
        yield return new WaitUntil(() => fadeDone);

        if (withLoadingPanel)
        {
            if (loadingPanelPrefab == null)
            {
                Debug.LogError("UIManager: loadingPanelPrefab reference is missing.");
                SceneManager.LoadScene(sceneType.ToString());
                yield break;
            }

            // 2. 검은 화면에서 로딩 패널 즉시 등장
            OpenPopupWithEffects("LOADING");
            loadingPanelPrefab.gameObject.SetActive(true);
            loadingPanelPrefab.SetProgress(0f);

            // 3. 화면 다시 밝게
            fadeDone = false;
            FadeIn(0.5f, () => fadeDone = true);
            yield return new WaitUntil(() => fadeDone);
        }

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneType.ToString());
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            if (withLoadingPanel) loadingPanelPrefab.SetProgress(asyncOperation.progress);
            yield return null;
        }

        if (withLoadingPanel) loadingPanelPrefab.SetProgress(1f);

        // 4. 화면 검게
        fadeDone = false;
        FadeOut(0.5f, () => fadeDone = true);
        yield return new WaitUntil(() => fadeDone);

        asyncOperation.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncOperation.isDone);

        if (withLoadingPanel)
        {
            loadingPanelPrefab.gameObject.SetActive(false);
            CloseAllGlobalPopups();
        }

        // 5. 화면 다시 밝게 (OnSceneLoaded의 FadeIn과 중복되므로 제거)
    }
    private bool IsAnyGlobalPopupActive()
    {
        return (saveLoadPanel != null && saveLoadPanel.activeSelf)
            || (optionPanel != null && optionPanel.activeSelf)
            || (selectCharacterPanel != null && selectCharacterPanel.activeSelf)
            || (endingListPanel != null && endingListPanel.activeSelf);
    }

    private void CloseAllGlobalPopups()
    {
        if (selectCharacterPanel != null) selectCharacterPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (endingListPanel != null) endingListPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);

        activePopupCount = 0;
        if (dimBackground != null) dimBackground.SetActive(false);
        if (cinematicBars != null) cinematicBars.SetActive(false);
        if (globalWindowTitleText != null) globalWindowTitleText.text = string.Empty;
    }

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.DOFade(0f, duration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                fadeCanvasGroup.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.DOFade(1f, duration)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CloseAllGlobalPopups();
        GameObject sceneCanvasObj = GameObject.FindGameObjectWithTag("Canvas");
        if (sceneCanvasObj != null) hudCanvas = sceneCanvasObj.GetComponent<Canvas>();

        // inventoryPanel만 재탐색
        InventoryUI invUI = GameObject.FindAnyObjectByType<InventoryUI>(FindObjectsInactive.Include);
        if (invUI != null) inventoryPanel = invUI.gameObject;
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        FadeIn(1f, () =>
     {
         if (SoundManager.Instance != null)
             SoundManager.Instance.PlaySceneBGM(scene.name);
     });
    }

    protected override void OnSceneUnloaded(Scene scene) { }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= InitializeInGameUI;
        }
    }
}
