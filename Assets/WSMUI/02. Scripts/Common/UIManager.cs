using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

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

    //Test를 위하여 ESceneType.Game -> ESceneType.TestScene으로 변경
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != Constants.ESceneType.PrototypeGame.ToString()) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsAnyGlobalPopupActive()) return;
            if (gameOverPanel != null && gameOverPanel.activeSelf) return;
            // 보관함이 열려있다면 (인벤토리도 같이 열려있는 상태)
            if (storagePanel != null && storagePanel.activeSelf)
            {
                // 이 함수 하나로 storagePanel과 inventoryPanel이 동시에 꺼집니다.
                ToggleStorage();
            }
            // 보관함은 없고 인벤토리만 단독으로 열려있을 때
            else if (inventoryPanel != null && inventoryPanel.activeSelf)
            {
                ToggleInventory();
            }
            else if (pauseMenuPanel != null)
            {
                TogglePauseMenu();
            }
        }

        // 2. E 키 처리 (상호작용 및 닫기)
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 보관함이 열려있는 상태라면 닫아줌 (인벤토리도 같이 닫힘)
            if (storagePanel != null && storagePanel.activeSelf)
            {
                ToggleStorage();
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

    private void UpdateCursorState()
    {
        // 1. 메인 메뉴 등 게임 씬이 아닐 때는 무조건 커서 활성화
        if (SceneManager.GetActiveScene().name != Constants.ESceneType.PrototypeGame.ToString())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // 2. 게임 씬일 경우: 하나라도 열려 있으면 true
        bool inventoryActive = (inventoryPanel != null && inventoryPanel.activeSelf);
        bool storageActive = (storagePanel != null && storagePanel.activeSelf);
        bool pauseActive = (pauseMenuPanel != null && pauseMenuPanel.activeSelf);
        bool gameOverActive = (gameOverPanel != null && gameOverPanel.activeSelf);

        // IsAnyPopupOpen(전역 팝업 카운트) + 로컬 패널들 상태 합산
        bool showCursor = IsAnyPopupOpen || inventoryActive || storageActive || pauseActive || gameOverActive;

        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // 모든 UI가 닫혀 있을 때만 커서를 잠금
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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
        UpdateCursorState();

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            SoundManager.Instance?.PauseBGM();
        }
        else
        {
            SoundManager.Instance?.ResumeBGM();
        }
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
        UpdateCursorState();

        if (_hudController == null)
        {
            _hudController = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
        }

        if (_hudController != null)
        {
            _hudController.gameObject.SetActive(!isNowActive);
        }
    }


    public void ToggleStorage(InventorySlot[] slots = null)
    {

        if (storagePanel == null || inventoryPanel == null) return;

        // 현재 보관함 상태의 반대로 설정 (열려있으면 닫고, 닫혀있으면 엶)
        bool isNowActive = !storagePanel.activeSelf;

        // 보관함과 인벤토리를 동시에 활성화/비활성화
        storagePanel.SetActive(isNowActive);
        inventoryPanel.SetActive(isNowActive);
        UpdateCursorState();

        if (isNowActive && slots != null)
        {
            InventoryManager.Instance.OpenStorage(slots);
        }
        else if (!isNowActive)
        {
            InventoryManager.Instance.OpenStorage(null); // CloseStorage() 역할
        }

        // HUD 제어 (두 창 중 하나라도 열려있으면 HUD는 숨김)
        if (_hudController == null)
            _hudController = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);

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

            // 2. 검은 화면에서 로딩 패널 즉시 등장
            OpenPopupWithEffects("로딩 중...");
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

        StorageUI storUI = GameObject.FindAnyObjectByType<StorageUI>(FindObjectsInactive.Include);
        if (storUI != null) storagePanel = storUI.gameObject;
        if (storagePanel != null) storagePanel.SetActive(false);
        bool isGameScene = scene.name == Constants.ESceneType.PrototypeGame.ToString();

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

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            // 현재 UI 상태에 맞춰 커서 상태를 강제로 재설정
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == Constants.ESceneType.PrototypeGame.ToString())
            {
                UpdateCursorState();
            }
        }
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