using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections.Generic;

public class GraphicOptions : MonoBehaviour
{
    [Header("해상도 화살표 UI")]
    [SerializeField] private Button resLeftButton;
    [SerializeField] private Button resRightButton;
    [SerializeField] private TextMeshProUGUI resValueText;

    [Header("화면 모드 화살표 UI")]
    [SerializeField] private Button modeLeftButton;
    [SerializeField] private Button modeRightButton;
    [SerializeField] private TextMeshProUGUI modeValueText;

    [Header("밝기/감마 설정 UI")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessText;
    [SerializeField] private TextMeshProUGUI brightnessSampleText;

    private Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    private List<Resolution> systemResolutions = new List<Resolution>();
    private List<string> modeOptions = new List<string> { "전체 화면", "전체 창 모드", "창 모드" };

    private int currentResIndex = 0;
    private int currentModeIndex = 0;

    public void Initialize()
    {
        this.gameObject.SetActive(true);

        brightnessSlider.wholeNumbers = true;
        brightnessSlider.minValue = 1;
        brightnessSlider.maxValue = 5;

        // [수정] 볼륨을 찾고 밝기를 적용하는 로직을 별도 함수로 분리하여 호출
        ApplySavedBrightnessToCurrentScene();

        int savedBrightnessLevel = PlayerPrefs.GetInt("Graphic_Brightness_Level", 3);
        savedBrightnessLevel = Mathf.Clamp(savedBrightnessLevel, 1, 5);

        brightnessSlider.value = savedBrightnessLevel;
        UpdateBrightnessUI(savedBrightnessLevel);

        brightnessSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.AddListener(SetBrightness);

        // --- 해상도 & 화면 모드 초기화 (기존 동일) ---
        systemResolutions.Clear();
        Resolution[] resolutions = Screen.resolutions;

        HashSet<string> uniqueResSet = new HashSet<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resStr = $"{resolutions[i].width}x{resolutions[i].height}";
            if (!uniqueResSet.Contains(resStr))
            {
                uniqueResSet.Add(resStr);
                systemResolutions.Add(resolutions[i]);
            }
        }

        currentResIndex = PlayerPrefs.GetInt("Graphic_ResolutionIndex", systemResolutions.Count - 1);
        if (currentResIndex >= systemResolutions.Count) currentResIndex = systemResolutions.Count - 1;
        UpdateResolutionDisplay();

        currentModeIndex = PlayerPrefs.GetInt("Graphic_DisplayMode", 0);
        UpdateDisplayModeDisplay();

        resLeftButton.onClick.RemoveAllListeners();
        resLeftButton.onClick.AddListener(() => ChangeResolutionIndex(-1));
        resRightButton.onClick.RemoveAllListeners();
        resRightButton.onClick.AddListener(() => ChangeResolutionIndex(1));

        modeLeftButton.onClick.RemoveAllListeners();
        modeLeftButton.onClick.AddListener(() => ChangeDisplayModeIndex(-1));
        modeRightButton.onClick.RemoveAllListeners();
        modeRightButton.onClick.AddListener(() => ChangeDisplayModeIndex(1));
    }

    // [핵심 추가] 씬이 바뀔 때마다 UIManager가 원격으로 호출해 줄 자동 적용 함수
    public void ApplySavedBrightnessToCurrentScene()
    {
        globalVolume = null;
        Volume[] allVolumes = FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Volume v in allVolumes)
        {
            if (v.isGlobal)
            {
                globalVolume = v;
                break;
            }
        }

        if (globalVolume != null)
        {
            if (!globalVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments = globalVolume.profile.Add<ColorAdjustments>(false);
            }

            colorAdjustments.active = true;
            colorAdjustments.postExposure.overrideState = true;

            int savedLevel = PlayerPrefs.GetInt("Graphic_Brightness_Level", 3);
            savedLevel = Mathf.Clamp(savedLevel, 1, 5);
            colorAdjustments.postExposure.value = (savedLevel - 3) * 0.8f;
        }
    }

    private void ChangeResolutionIndex(int direction)
    {
        currentResIndex += direction;
        if (currentResIndex < 0) currentResIndex = systemResolutions.Count - 1;
        else if (currentResIndex >= systemResolutions.Count) currentResIndex = 0;
        UpdateResolutionDisplay();
        ApplyResolution(currentResIndex);
    }

    private void UpdateResolutionDisplay()
    {
        if (currentResIndex >= 0 && currentResIndex < systemResolutions.Count)
        {
            Resolution res = systemResolutions[currentResIndex];
            resValueText.text = $"{res.width} x {res.height}";
        }
    }

    private void ApplyResolution(int index)
    {
        if (index >= 0 && index < systemResolutions.Count)
        {
            Resolution res = systemResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        }
    }

    private void ChangeDisplayModeIndex(int direction)
    {
        currentModeIndex += direction;
        if (currentModeIndex < 0) currentModeIndex = modeOptions.Count - 1;
        else if (currentModeIndex >= modeOptions.Count) currentModeIndex = 0;
        UpdateDisplayModeDisplay();
        ApplyDisplayMode(currentModeIndex);
    }

    private void UpdateDisplayModeDisplay()
    {
        if (currentModeIndex >= 0 && currentModeIndex < modeOptions.Count)
            modeValueText.text = modeOptions[currentModeIndex];
    }

    private void ApplyDisplayMode(int index)
    {
        switch (index)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 1: Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break;
            case 2: Screen.fullScreenMode = FullScreenMode.Windowed; break;
        }
    }

    // --- 밝기(정수 스냅) 제어 로직 ---
    private void SetBrightness(float value)
    {
        int step = Mathf.RoundToInt(value);
        UpdateBrightnessUI(step);

        if (colorAdjustments != null)
        {
            colorAdjustments.active = true;
            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.postExposure.value = (step - 3) * 0.8f;
        }

        if (brightnessSampleText != null)
        {
            // 3단계(기본)일 때 RGB 0.12 (완전 검은 배경에서 '겨우 보이는' 수준의 극어두운 회색)
            Color baseGray = new Color(0.12f, 0.12f, 0.12f, 1f);

            // 단계별 스케일 폭을 키움 (0.25 -> 0.5)
            // 1단계(0배 = 완전블랙), 3단계(1배 = 겨우보임), 5단계(2배 = 선명해짐)
            float rgbScale = 1f + (step - 3) * 0.5f;

            Color simulatedColor = baseGray * rgbScale;
            simulatedColor.a = 1f; // 알파(투명도)는 100% 유지

            brightnessSampleText.color = simulatedColor;
        }
    }

    private void UpdateBrightnessUI(int step)
    {
        if (brightnessText != null) brightnessText.text = $"{step}";
    }

    // [추가] 그래픽 설정 기본값으로 되돌리기
    public void ResetToDefault()
    {
        // 1. 밝기 기본값 (3단계)
        brightnessSlider.value = 3;

        // 2. 해상도 기본값 (지원하는 가장 큰 해상도 = 배열의 마지막)
        if (systemResolutions.Count > 0)
        {
            currentResIndex = systemResolutions.Count - 1;
            UpdateResolutionDisplay();
            ApplyResolution(currentResIndex);
        }

        // 3. 화면 모드 기본값 (전체 화면 = 0번)
        currentModeIndex = 0;
        UpdateDisplayModeDisplay();
        ApplyDisplayMode(currentModeIndex);

        Debug.Log("[GraphicOptions] 그래픽 설정이 기본값으로 초기화 대기 중입니다.");
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetInt("Graphic_ResolutionIndex", currentResIndex);
        PlayerPrefs.SetInt("Graphic_DisplayMode", currentModeIndex);
        PlayerPrefs.SetInt("Graphic_Brightness_Level", Mathf.RoundToInt(brightnessSlider.value));
    }

    public void RevertOptions()
    {
        Initialize();
        ApplyResolution(currentResIndex);
        ApplyDisplayMode(currentModeIndex);
    }
}