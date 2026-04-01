using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : Singleton<UIManager>
{
    public Canvas Canvas => GetCanvas();

    [Header("UI Panels")]
    public GameObject inventoryPanel; // 씬 전환 시 참조를 다시 할당할 수 있도록 public 오픈

    // HUD 초기화 처리를 위한 참조
    private HUDController _hudController;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += InitializeInGameUI;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == Constants.ESceneType.Game.ToString())
        {
            if (inventoryPanel != null && (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab)))
            {
                ToggleInventory();
            }
        }
    }

    private void ToggleInventory()
    {
        bool isPanelActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isPanelActive);

        //커서 상태 나중에 수정
        // if (!isPanelActive)
        // {
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = true;
        // }
        // else
        // {
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }
    }

    private void InitializeInGameUI()
    {
        if (_hudController == null)
            _hudController = FindFirstObjectByType<HUDController>();

        if (_hudController != null)
        {
            _hudController.InitHUD();
            Debug.Log("UIManager: HUD 초기화 완료");
        }
    }

    public void LoadScene(Constants.ESceneType sceneType)
    {
        StartCoroutine(LoadSceneAsync(sceneType));
    }

    private IEnumerator LoadSceneAsync(Constants.ESceneType sceneType)
    {
        var loadingPanelPrefab = Resources.Load<GameObject>("Loading Panel");
        if (loadingPanelPrefab == null)
        {
            Debug.LogError("Loading Panel 프리팹을 찾을 수 없습니다.");
            yield break;
        }

        var loadingPanelObject = Instantiate(loadingPanelPrefab, Canvas.transform);
        var loadingPanelController = loadingPanelObject.GetComponent<LoadingPanelController>();

        bool showDone = false;
        loadingPanelController.Show(() => showDone = true);
        yield return new WaitUntil(() => showDone);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneType.ToString());
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            loadingPanelController.SetProgress(asyncOperation.progress);
            yield return null;
        }

        loadingPanelController.SetProgress(1f);
        asyncOperation.allowSceneActivation = true;

        yield return new WaitUntil(() => asyncOperation.isDone);

        Destroy(loadingPanelObject);
    }

    private Canvas GetCanvas()
    {
        var canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.tag = "Canvas";

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            return canvas;
        }
        return canvasObject.GetComponent<Canvas>();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _hudController = FindFirstObjectByType<HUDController>();
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