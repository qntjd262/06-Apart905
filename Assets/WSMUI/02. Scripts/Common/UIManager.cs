using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Canvases")]
    [SerializeField] private Canvas globalCanvas;
    [SerializeField] private Canvas hudCanvas;
    [SerializeField] private GameObject loadingPanelPrefab;

    public Canvas CurrentCanvas => hudCanvas != null ? hudCanvas : globalCanvas;

    [Header("Global Effects")]
    [SerializeField] private GameObject dimBackground;
    [SerializeField] private GameObject cinematicBars;
    [SerializeField] private TextMeshProUGUI globalWindowTitleText;

    [Header("UI Panels (Local)")]
    public GameObject inventoryPanel;
    public GameObject pauseMenuPanel;

    [Header("Global Popup Panels")]
    public GameObject selectCharacterPanel;
    public GameObject saveLoadPanel;
    public GameObject endingListPanel;
    public GameObject optionPanel;

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

        bool isPaused = pauseMenuPanel != null && pauseMenuPanel.activeSelf;
        bool isBlockedByPopup = IsAnyGlobalPopupActive();
        if (!isPaused && !isBlockedByPopup && (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab)))
        {
            if (inventoryPanel != null) ToggleInventory();
        }
    }

    public void OpenSelectCharacterPanel()
    {
        if (selectCharacterPanel != null)
        {
            selectCharacterPanel.SetActive(true);
        }
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
            OpenPopupWithEffects("ENDING LIST");
        }
    }

    public void OpenOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
        }
    }

    public void RegisterGlobalEffects(GameObject dim, GameObject bars, TextMeshProUGUI title)
    {
        if (dimBackground != null && dimBackground != dim)
        {
            dimBackground.SetActive(false);
        }

        if (cinematicBars != null && cinematicBars != bars)
        {
            cinematicBars.SetActive(false);
        }

        if (globalWindowTitleText != null && globalWindowTitleText != title)
        {
            globalWindowTitleText.text = string.Empty;
        }

        dimBackground = dim;
        cinematicBars = bars;
        globalWindowTitleText = title;

        if (dimBackground != null) dimBackground.SetActive(false);
        if (cinematicBars != null) cinematicBars.SetActive(false);
        if (globalWindowTitleText != null) globalWindowTitleText.text = string.Empty;
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

    public void LoadScene(Constants.ESceneType sceneType)
    {
        StartCoroutine(LoadSceneAsync(sceneType));
    }

    private void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("UIManager: inventoryPanel reference is missing.");
            return;
        }

        bool isNowActive = !inventoryPanel.activeSelf;
        Debug.Log($"UIManager: Inventory toggle -> {isNowActive} ({inventoryPanel.name})");

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

    private IEnumerator LoadSceneAsync(Constants.ESceneType sceneType)
    {
        Time.timeScale = 1f;

        DestroyExistingLoadingPanels();

        bool loadingEffectsOpened = dimBackground != null || cinematicBars != null || globalWindowTitleText != null;
        if (loadingEffectsOpened)
        {
            OpenPopupWithEffects("LOADING");
        }

        if (loadingPanelPrefab == null)
        {
            Debug.LogError("UIManager: loadingPanelPrefab reference is missing. Loading without the loading panel.");
            if (loadingEffectsOpened) ClosePopupWithEffects();
            SceneManager.LoadScene(sceneType.ToString());
            yield break;
        }

        if (CurrentCanvas == null)
        {
            Debug.LogError("UIManager: CurrentCanvas is null. Loading without the loading panel.");
            if (loadingEffectsOpened) ClosePopupWithEffects();
            SceneManager.LoadScene(sceneType.ToString());
            yield break;
        }

        GameObject loadingPanelObject = Instantiate(loadingPanelPrefab);
        DontDestroyOnLoad(loadingPanelObject);
        if (!loadingPanelObject.activeSelf)
        {
            loadingPanelObject.SetActive(true);
        }

        Canvas loadingCanvas = loadingPanelObject.GetComponent<Canvas>();
        if (loadingCanvas == null)
        {
            loadingCanvas = loadingPanelObject.AddComponent<Canvas>();
        }

        loadingCanvas.overrideSorting = true;
        loadingCanvas.sortingOrder = 1000;

        if (loadingPanelObject.GetComponent<GraphicRaycaster>() == null)
        {
            loadingPanelObject.AddComponent<GraphicRaycaster>();
        }

        loadingPanelObject.transform.SetAsLastSibling();

        LoadingPanelController loadingPanelController = loadingPanelObject.GetComponent<LoadingPanelController>();
        if (loadingPanelController == null)
        {
            Debug.LogError("UIManager: LoadingPanelController is missing on the loading panel prefab.");
            Destroy(loadingPanelObject);
            if (loadingEffectsOpened) ClosePopupWithEffects();
            SceneManager.LoadScene(sceneType.ToString());
            yield break;
        }

        bool showDone = false;
        loadingPanelController.Show(() => showDone = true);
        Debug.Log("UIManager: Loading panel show requested. Waiting for fade-in to finish.");
        yield return new WaitUntil(() => showDone);

        Debug.Log("UIManager: Loading panel fade-in completed. Starting async scene load.");
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneType.ToString());
        if (asyncOperation == null)
        {
            Debug.LogError($"UIManager: Failed to start async loading for scene '{sceneType}'.");
            Destroy(loadingPanelObject);
            if (loadingEffectsOpened) ClosePopupWithEffects();
            yield break;
        }

        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            loadingPanelController.SetProgress(asyncOperation.progress);
            yield return null;
        }

        loadingPanelController.SetProgress(1f);
        asyncOperation.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncOperation.isDone);

        CloseAllGlobalPopups();
        Destroy(loadingPanelObject);
    }

    private bool IsAnyGlobalPopupActive()
    {
        return (saveLoadPanel != null && saveLoadPanel.activeSelf)
            || (optionPanel != null && optionPanel.activeSelf)
            || (selectCharacterPanel != null && selectCharacterPanel.activeSelf)
            || (endingListPanel != null && endingListPanel.activeSelf);
    }

    private GameObject GetPopupRoot(GameObject popupObject)
    {
        if (popupObject == null) return null;

        Transform current = popupObject.transform;
        while (current.parent != null && current.parent.GetComponent<Canvas>() == null)
        {
            current = current.parent;
        }

        return current.gameObject;
    }

    private void DestroyExistingLoadingPanels()
    {
        LoadingPanelController[] loadingPanels = FindObjectsByType<LoadingPanelController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (LoadingPanelController panel in loadingPanels)
        {
            if (panel != null)
            {
                Destroy(panel.gameObject);
            }
        }
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

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CloseAllGlobalPopups();

        GameObject sceneCanvasObj = GameObject.FindGameObjectWithTag("Canvas");
        if (sceneCanvasObj != null)
        {
            hudCanvas = sceneCanvasObj.GetComponent<Canvas>();
        }

        InventoryUI invUI = GameObject.FindAnyObjectByType<InventoryUI>(FindObjectsInactive.Include);
        if (invUI != null)
        {
            inventoryPanel = invUI.gameObject;
        }

        PauseMenuController pauseUI = GameObject.FindAnyObjectByType<PauseMenuController>(FindObjectsInactive.Include);
        if (pauseUI != null)
        {
            pauseMenuPanel = pauseUI.gameObject;
        }

        CharacterSelector characterSelector = GameObject.FindAnyObjectByType<CharacterSelector>(FindObjectsInactive.Include);
        if (characterSelector != null)
        {
            selectCharacterPanel = GetPopupRoot(characterSelector.gameObject);
        }

        SaveLoadController saveLoadController = GameObject.FindAnyObjectByType<SaveLoadController>(FindObjectsInactive.Include);
        if (saveLoadController != null)
        {
            saveLoadPanel = GetPopupRoot(saveLoadController.gameObject);
        }

        OptionsController optionsController = GameObject.FindAnyObjectByType<OptionsController>(FindObjectsInactive.Include);
        if (optionsController != null)
        {
            optionPanel = GetPopupRoot(optionsController.gameObject);
        }

        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (selectCharacterPanel != null) selectCharacterPanel.SetActive(false);
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);

        CloseAllGlobalPopups();
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
