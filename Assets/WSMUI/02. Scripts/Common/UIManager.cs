using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] public GameObject storagePanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject dialoguePanel;

    [Header("Global Popup Panels")]
    [SerializeField] private GameObject selectCharacterPanel;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject endingListPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private LoadingPanelController loadingPanelPrefab;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("UI Stack Management")]
    private List<GameObject> _activeUIStack = new List<GameObject>();
    private bool _isSceneLoading = false;

    private HUDController _hudController;
    private int activePopupCount = 0;
    public bool IsAnyPopupOpen => activePopupCount > 0;
    public bool IsAnyUIOpen => _activeUIStack.Count > 0;

    [Header("Interaction UI")]
    private InteractUI activeInteractUI;
    [SerializeField] private Sprite[] interactIcons;

    private QuestTrackerUI _cachedTracker;
    public QuestTrackerUI MainTracker => _cachedTracker;

    private float _lastStorageToggleTime;

    public SaveLoadController saveLoadController;

    // [최적화 핵심] 매 프레임 문자열 비교를 방지하기 위한 캐싱 변수
    private bool _isCurrentSceneGame = false;

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
        // [최적화] 매 프레임 스트링을 생성하던 무거운 코드를 단순 bool 체크로 변경
        if (!_isCurrentSceneGame) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SlotUI.PickedSlot != null) SlotUI.PickedSlot.CancelPick();
            if (_activeUIStack.Count > 0)
            {
                GameObject topUI = _activeUIStack[_activeUIStack.Count - 1];
                if (topUI == gameOverPanel) return;
                else if (topUI == storagePanel) ToggleStorage();
                else if (topUI == inventoryPanel) ToggleInventory();
                else if (topUI == pauseMenuPanel) TogglePauseMenu();
                else if (topUI == dialoguePanel) { }
                else if (topUI == optionPanel)
                {
                    topUI.GetComponent<OptionsController>().OnReturnClick();
                }
                else if (topUI == saveLoadPanel)
                {
                    if (saveLoadController != null)
                    {
                        System.Action closeAction = saveLoadController.onCloseAction;
                        saveLoadController.onCloseAction = null;

                        UnregisterUI(topUI);
                        topUI.SetActive(false);

                        closeAction?.Invoke();
                    }
                }
                else
                {
                    UnregisterUI(topUI);
                    topUI.SetActive(false);
                }
                return;
            }
            TogglePauseMenu();
        }

        if (InputManager.Instance.GetKeyDown(EKeyAction.Inventory) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (InputManager.Instance.GetKeyDown(EKeyAction.Interact))
        {
            if (storagePanel != null && storagePanel.activeSelf)
            {
                ToggleStorage();
            }
        }
    }

    public void RegisterUI(GameObject uiPanel)
    {
        if (uiPanel == null) return;

        if (_activeUIStack.Contains(uiPanel))
        {
            _activeUIStack.Remove(uiPanel);
        }
        _activeUIStack.Add(uiPanel);

        UpdateCursorState();
    }

    public void UnregisterUI(GameObject uiPanel)
    {
        if (uiPanel == null) return;

        if (_activeUIStack.Contains(uiPanel))
        {
            _activeUIStack.Remove(uiPanel);
        }

        UpdateCursorState();
    }

    public void UpdateCursorState()
    {
        if (_isSceneLoading) return;

        _activeUIStack.RemoveAll(ui => ui == null || !ui.activeSelf);

        // [최적화] 여기서도 문자열 비교 대신 미리 구해둔 bool 값을 활용
        if (!_isCurrentSceneGame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        bool showCursor = _activeUIStack.Count > 0;

        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetHUDActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetHUDActive(true);
        }
    }

    private void SetHUDActive(bool isActive)
    {
        if (_hudController == null) _hudController = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
        if (_hudController != null) _hudController.gameObject.SetActive(isActive);

        if (_cachedTracker != null)
        {
            _cachedTracker.gameObject.SetActive(isActive);
        }
    }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        if (_isSceneLoading) return;

        // [최적화] 에디터용 렉 유발 스트링 체크 코드 수정
        if (_isCurrentSceneGame)
        {
            if (_activeUIStack.Count == 0 && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
#endif

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;
        if (storagePanel != null && storagePanel.activeSelf)
        {
            ToggleStorage();
            return;
        }

        bool isNowActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isNowActive);

        PlayerMove playerMove = FindFirstObjectByType<PlayerMove>();

        if (isNowActive)
        {
            inventoryPanel.GetComponent<InventoryUI>().SetQuestSelectorActive(true);
            RegisterUI(inventoryPanel);
            playerMove.PausePlayer();
            Debug.Log("인벤 Open");
        }
        else
        {
            UnregisterUI(inventoryPanel);
        }
    }

    public void ToggleStorage(InventorySlot[] slots = null)
    {
        if (storagePanel == null || inventoryPanel == null) return;

        if (Time.unscaledTime - _lastStorageToggleTime < 0.1f) return;
        _lastStorageToggleTime = Time.unscaledTime;

        bool isNowActive = !storagePanel.activeSelf;

        storagePanel.SetActive(isNowActive);
        inventoryPanel.SetActive(isNowActive);

        PlayerMove playerMove = FindFirstObjectByType<PlayerMove>();

        if (isNowActive)
        {
            inventoryPanel.GetComponent<InventoryUI>().SetQuestSelectorActive(false);
            RegisterUI(inventoryPanel);
            RegisterUI(storagePanel);

            if (slots != null) InventoryManager.Instance.OpenStorage(slots);
            playerMove.PausePlayer();
            Debug.Log("인벤 Open");
        }
        else
        {
            UnregisterUI(storagePanel);
            UnregisterUI(inventoryPanel);
            InventoryManager.Instance.OpenStorage(null);
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuPanel == null) return;
        bool isPaused = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isPaused);

        if (isPaused)
        {
            RegisterUI(pauseMenuPanel);
            GameManager.Instance?.Pause();
        }
        else
        {
            UnregisterUI(pauseMenuPanel);
            GameManager.Instance?.Resume();
        }
    }

    public void ShowPauseMenuWithoutChangingTimeScale()
    {
        if (pauseMenuPanel != null && !pauseMenuPanel.activeSelf)
        {
            pauseMenuPanel.SetActive(true);
            RegisterUI(pauseMenuPanel);
        }
    }
    public void OpenSelectCharacterPanel()
    {
        if (selectCharacterPanel != null)
        {
            selectCharacterPanel.SetActive(true);
            RegisterUI(selectCharacterPanel);
        }
    }

    public void CloseSelectCharacterPanel()
    {
        if (selectCharacterPanel != null)
        {
            selectCharacterPanel.SetActive(false);
            UnregisterUI(selectCharacterPanel);
        }
    }

    public void OpenSaveLoadPanelAsLoadMode()
    {
        if (saveLoadPanel != null)
        {
            SaveLoadController controller = saveLoadPanel.GetComponent<SaveLoadController>();
            if (controller != null) controller.currentMode = Constants.ESaveLoadType.Load;

            saveLoadPanel.SetActive(true);
            RegisterUI(saveLoadPanel);
        }
    }

    public void OpenEndingListPanel()
    {
        if (endingListPanel != null)
        {
            endingListPanel.SetActive(true);
            RegisterUI(endingListPanel);
        }
    }

    public void OpenOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            RegisterUI(optionPanel);
        }
    }
    public void OpenGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            GameOverController ctrl = gameOverPanel.GetComponent<GameOverController>();
            if (ctrl != null) ctrl.Open();
        }
        else
        {
            Debug.LogError("UIManager에 gameOverPanel이 연결되지 않았습니다!");
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

    private void CloseAllGlobalPopups()
    {
        if (selectCharacterPanel != null && selectCharacterPanel.activeSelf) CloseSelectCharacterPanel();
        if (saveLoadPanel != null && saveLoadPanel.activeSelf) { saveLoadPanel.SetActive(false); UnregisterUI(saveLoadPanel); }
        if (endingListPanel != null && endingListPanel.activeSelf) { endingListPanel.SetActive(false); UnregisterUI(endingListPanel); }
        if (optionPanel != null && optionPanel.activeSelf) { optionPanel.SetActive(false); UnregisterUI(optionPanel); }

        activePopupCount = 0;
        if (dimBackground != null) dimBackground.SetActive(false);
        if (cinematicBars != null) cinematicBars.SetActive(false);
        if (globalWindowTitleText != null) globalWindowTitleText.text = string.Empty;
    }

    public void LoadScene(Constants.ESceneType sceneType, bool withLoadingPanel = true)
    {
        StartCoroutine(LoadSceneAsync(sceneType, withLoadingPanel));
    }

    private IEnumerator LoadSceneAsync(Constants.ESceneType sceneType, bool withLoadingPanel = true)
    {
        Time.timeScale = 1f;
        _isSceneLoading = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        bool fadeDone = false;
        SoundManager.Instance?.StopBGM();
        FadeOut(0.5f, () => fadeDone = true);
        yield return new WaitUntil(() => fadeDone);

        if (withLoadingPanel)
        {
            if (loadingPanelPrefab == null)
            {
                SceneManager.LoadScene(sceneType.ToString());
                yield break;
            }

            OpenPopupWithEffects("로딩 중...");
            loadingPanelPrefab.gameObject.SetActive(true);
            loadingPanelPrefab.SetProgress(0f);

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

        if (withLoadingPanel)
        {
            loadingPanelPrefab.SetProgress(1f);

            fadeDone = false;
            FadeOut(0.5f, () => fadeDone = true);
            yield return new WaitUntil(() => fadeDone);

            loadingPanelPrefab.gameObject.SetActive(false);
            CloseAllGlobalPopups();
        }

        asyncOperation.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncOperation.isDone);

        _isSceneLoading = false;
        UpdateCursorState();
    }

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        fadeCanvasGroup.DOKill();
        fadeCanvasGroup.gameObject.SetActive(true);

        fadeCanvasGroup.DOFade(0f, duration).SetUpdate(true).OnComplete(() =>
        {
            fadeCanvasGroup.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        fadeCanvasGroup.DOKill();
        fadeCanvasGroup.gameObject.SetActive(true);

        fadeCanvasGroup.DOFade(1f, duration).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _activeUIStack.Clear();
        CloseAllGlobalPopups();

        _isCurrentSceneGame = scene.name == Constants.ESceneType.PrototypeGame.ToString();
        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = FindFirstObjectByType<CanvasGroup>(FindObjectsInactive.Include);
        }

        GameObject sceneCanvasObj = GameObject.FindGameObjectWithTag("Canvas");
        if (sceneCanvasObj != null) hudCanvas = sceneCanvasObj.GetComponent<Canvas>();

        InventoryUI invUI = GameObject.FindAnyObjectByType<InventoryUI>(FindObjectsInactive.Include);
        if (invUI != null) inventoryPanel = invUI.gameObject;
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        StorageUI storUI = GameObject.FindAnyObjectByType<StorageUI>(FindObjectsInactive.Include);
        if (storUI != null) storagePanel = storUI.gameObject;
        if (storagePanel != null) storagePanel.SetActive(false);
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (_isCurrentSceneGame)
        {
            UpdateCursorState();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        FadeIn(1f, () =>
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySceneBGM(scene.name);
        });

        GraphicOptions graphicOpt = FindFirstObjectByType<GraphicOptions>(FindObjectsInactive.Include);
        if (graphicOpt != null) graphicOpt.ApplySavedBrightnessToCurrentScene();
    }

    private void InitializeInGameUI()
    {
        if (_hudController == null) _hudController = FindFirstObjectByType<HUDController>();
        if (_hudController != null) _hudController.InitHUD();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (_isCurrentSceneGame)
            {
                UpdateCursorState();
            }
        }
    }

    public void RegisterTracker(QuestTrackerUI tracker) { _cachedTracker = tracker; }
    public void RegisterInteractionUI(InteractUI ui) { activeInteractUI = ui; activeInteractUI.gameObject.SetActive(false); }
    public void ShowInteractUI(string text, Constants.InteractType type)
    {
        if (activeInteractUI == null) return;
        activeInteractUI.gameObject.SetActive(true);

        KeyCode currentKey = InputManager.Instance.GetKeyForAction(EKeyAction.Interact);
        activeInteractUI.interactText.text = $"[{currentKey.ToString()}]를 눌러 {text}";
        activeInteractUI.iconImage.sprite = interactIcons[(int)type];
    }
    public void HideInteractUI() { if (activeInteractUI == null) return; activeInteractUI.gameObject.SetActive(false); }

    protected override void OnSceneUnloaded(Scene scene) { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (GameManager.Instance != null) GameManager.Instance.OnGameStart -= InitializeInGameUI;
    }

    public KeyCode GetKeyForAction(EKeyAction action)
    {
        if (InputManager.Instance != null)
        {
            return InputManager.Instance.GetKeyForAction(action);
        }
        return KeyCode.None;
    }
}