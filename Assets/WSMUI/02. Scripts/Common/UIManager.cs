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
    // 현재 화면에 켜져 있는 UI들을 순서대로 담아두는 리스트 (스택 역할)
    private List<GameObject> _activeUIStack = new List<GameObject>();
    private bool _isSceneLoading = false; // 씬 로딩 중 커서 제어 방어용

    private HUDController _hudController;
    private int activePopupCount = 0;
    public bool IsAnyPopupOpen => activePopupCount > 0;

    [Header("Interaction UI")]
    private InteractUI activeInteractUI;
    [SerializeField] private Sprite[] interactIcons;

    private QuestTrackerUI _cachedTracker;
    public QuestTrackerUI MainTracker => _cachedTracker;

    private float _lastStorageToggleTime;

    public SaveLoadController saveLoadController;

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
        if (SceneManager.GetActiveScene().name != Constants.ESceneType.PrototypeGame.ToString()) return;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (SlotUI.PickedSlot != null)
            {
                SlotUI.PickedSlot.CancelPick();
            }

            if (_activeUIStack.Count > 0)
            {
                GameObject topUI = _activeUIStack[_activeUIStack.Count - 1];
                if (topUI == gameOverPanel) return;
                else if (topUI == storagePanel) ToggleStorage();
                else if (topUI == inventoryPanel) ToggleInventory();
                else if (topUI == pauseMenuPanel) TogglePauseMenu();
                else if (topUI == dialoguePanel) { /* DialogueManager 등에서 닫도록 처리 */ }
                else
                {
                    UnregisterUI(topUI);
                    topUI.SetActive(false);
                }

                return; // 창을 하나 닫았으면 프레임 종료 (여러 개 동시 닫힘 방지)
            }

            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.E))
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
        if (_isSceneLoading) return; // 씬 로딩 중에는 상태 변경 차단

        _activeUIStack.RemoveAll(ui => ui == null || !ui.activeSelf);
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != Constants.ESceneType.PrototypeGame.ToString())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // 스택에 열려있는 창이 1개라도 있으면 커서 활성화, 없으면 비활성화
        bool showCursor = _activeUIStack.Count > 0;

        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        if (_isSceneLoading) return;

        if (SceneManager.GetActiveScene().name == Constants.ESceneType.PrototypeGame.ToString())
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

        if (isNowActive)
        {
            inventoryPanel.GetComponent<InventoryUI>().SetQuestSelectorActive(true);
            RegisterUI(inventoryPanel);
        }
        else
        {
            UnregisterUI(inventoryPanel);
        }

        if (_cachedTracker != null) _cachedTracker.gameObject.SetActive(!isNowActive);

        if (_hudController == null) _hudController = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
        if (_hudController != null) _hudController.gameObject.SetActive(!isNowActive);
    }

    public void ToggleStorage(InventorySlot[] slots = null)
    {
        if (storagePanel == null || inventoryPanel == null) return;

        // [수정 3 핵심] 0.1초 이내에 토글이 연달아 호출되면 무시 (E키 충돌 방지)
        if (Time.unscaledTime - _lastStorageToggleTime < 0.1f) return;
        _lastStorageToggleTime = Time.unscaledTime;

        bool isNowActive = !storagePanel.activeSelf;

        storagePanel.SetActive(isNowActive);
        inventoryPanel.SetActive(isNowActive);

        if (isNowActive)
        {
            inventoryPanel.GetComponent<InventoryUI>().SetQuestSelectorActive(false);
            RegisterUI(inventoryPanel);
            RegisterUI(storagePanel);

            if (slots != null) InventoryManager.Instance.OpenStorage(slots);
        }
        else
        {
            UnregisterUI(storagePanel);
            UnregisterUI(inventoryPanel);
            InventoryManager.Instance.OpenStorage(null);
        }

        if (_cachedTracker != null) _cachedTracker.gameObject.SetActive(!isNowActive);

        if (_hudController == null) _hudController = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
        if (_hudController != null) _hudController.gameObject.SetActive(!isNowActive);
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuPanel == null) return;
        bool isPaused = !pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(isPaused);

        if (isPaused)
        {
            RegisterUI(pauseMenuPanel);
            SoundManager.Instance?.PauseBGM();
        }
        else
        {
            UnregisterUI(pauseMenuPanel);
            SoundManager.Instance?.ResumeBGM();
        }

        Time.timeScale = isPaused ? 0f : 1f;
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
                Debug.LogError("UIManager: loadingPanelPrefab reference is missing.");
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

        if (withLoadingPanel) loadingPanelPrefab.SetProgress(1f);

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

        _isSceneLoading = false;
        UpdateCursorState();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _activeUIStack.Clear(); // 씬이 로드되면 스택 초기화
        CloseAllGlobalPopups();

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

        bool isGameScene = scene.name == Constants.ESceneType.PrototypeGame.ToString();
        Time.timeScale = 1f;
        if (isGameScene)
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
    }

    private void InitializeInGameUI()
    {
        if (_hudController == null) _hudController = FindFirstObjectByType<HUDController>();
        if (_hudController != null) _hudController.InitHUD();
    }

    public void FadeIn(float duration = 0.5f, Action onComplete = null)
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.DOFade(0f, duration).SetUpdate(true).OnComplete(() =>
        {
            fadeCanvasGroup.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void FadeOut(float duration = 0.5f, Action onComplete = null)
    {
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.DOFade(1f, duration).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == Constants.ESceneType.PrototypeGame.ToString())
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
        activeInteractUI.interactText.text = $"[E]를 눌러 {text}";
        activeInteractUI.iconImage.sprite = interactIcons[(int)type];
    }
    public void HideInteractUI() { if (activeInteractUI == null) return; activeInteractUI.gameObject.SetActive(false); }

    protected override void OnSceneUnloaded(Scene scene) { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (GameManager.Instance != null) GameManager.Instance.OnGameStart -= InitializeInGameUI;
    }

#if UNITY_EDITOR
    // 화면 좌측 상단에 현재 스택의 상태를 실시간으로 그린다.
    private void OnGUI()
    {
        if (_activeUIStack.Count > 0)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 500, 20), "현재 스택에 남은 UI 수: " + _activeUIStack.Count);
            for (int i = 0; i < _activeUIStack.Count; i++)
            {
                if (_activeUIStack[i] != null)
                    GUI.Label(new Rect(10, 30 + (i * 20), 500, 20), "- " + _activeUIStack[i].name);
            }
        }
        else
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(10, 10, 500, 20), "스택 비어있음 (이 상태에서 커서가 보이면 유니티 에디터 버그임)");
        }
    }
#endif
}