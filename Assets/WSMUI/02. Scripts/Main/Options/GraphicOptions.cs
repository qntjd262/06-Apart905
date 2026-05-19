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
    
    [Header("밝기/감마 설정")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessText;
    [SerializeField] private Volume globalVolume;

    private ColorAdjustments colorAdjustments;
    private List<Resolution> systemResolutions = new List<Resolution>();
    private List<string> modeOptions = new List<string> { "전체 화면", "전체 창 모드", "창 모드" };

    private int currentResIndex = 0;
    private int currentModeIndex = 0;

    public void Initialize()
    {
        // 1. 밝기 설정 초기화
        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            float savedBrightness = PlayerPrefs.GetFloat("Graphic_Brightness", 1.0f);
            brightnessSlider.value = savedBrightness;
            if (brightnessText != null) brightnessText.text = $"{savedBrightness:F1}";
            colorAdjustments.postExposure.value = (savedBrightness - 1.0f) * 2f; 
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        // 2. 시스템 지원 해상도 리스트 구성
        systemResolutions.Clear();
        Resolution[] resolutions = Screen.resolutions;
        
        // 중복 해상도(주사율만 다른 경우)를 필터링하여 리스트 가독성 최적화
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

        // 3. 해상도 기본값 인덱스 매칭
        currentResIndex = systemResolutions.Count - 1; // 기본 최상위 해상도 지정
        for (int i = 0; i < systemResolutions.Count; i++)
        {
            if (systemResolutions[i].width == Screen.currentResolution.width &&
                systemResolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
                break;
            }
        }
        currentResIndex = PlayerPrefs.GetInt("Graphic_ResolutionIndex", currentResIndex);
        UpdateResolutionDisplay();

        // 4. 화면 모드 기본값 세팅
        currentModeIndex = PlayerPrefs.GetInt("Graphic_DisplayMode", 0);
        UpdateDisplayModeDisplay();

        // 5. 화살표 버튼 이벤트 리스너 바인딩 (Remove 후 재등록으로 중복 누적 방지)
        resLeftButton.onClick.RemoveAllListeners();
        resLeftButton.onClick.AddListener(() => ChangeResolutionIndex(-1));
        resRightButton.onClick.RemoveAllListeners();
        resRightButton.onClick.AddListener(() => ChangeResolutionIndex(1));

        modeLeftButton.onClick.RemoveAllListeners();
        modeLeftButton.onClick.AddListener(() => ChangeDisplayModeIndex(-1));
        modeRightButton.onClick.RemoveAllListeners();
        modeRightButton.onClick.AddListener(() => ChangeDisplayModeIndex(1));
    }

    // --- 해상도 제어 로직 ---
    private void ChangeResolutionIndex(int direction)
    {
        currentResIndex += direction;
        
        // 인덱스 범위 순환(Loop) 처리
        if (currentResIndex < 0) currentResIndex = systemResolutions.Count - 1;
        else if (currentResIndex >= systemResolutions.Count) currentResIndex = 0;

        PlayerPrefs.SetInt("Graphic_ResolutionIndex", currentResIndex);
        UpdateResolutionDisplay();

        // 물리 엔진 해상도 실시간 교체
        Resolution res = systemResolutions[currentResIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }

    private void UpdateResolutionDisplay()
    {
        if (currentResIndex >= 0 && currentResIndex < systemResolutions.Count)
        {
            Resolution res = systemResolutions[currentResIndex];
            resValueText.text = $"{res.width} x {res.height}";
        }
    }

    // --- 화면 모드 제어 로직 ---
    private void ChangeDisplayModeIndex(int direction)
    {
        currentModeIndex += direction;

        if (currentModeIndex < 0) currentModeIndex = modeOptions.Count - 1;
        else if (currentModeIndex >= modeOptions.Count) currentModeIndex = 0;

        PlayerPrefs.SetInt("Graphic_DisplayMode", currentModeIndex);
        UpdateDisplayModeDisplay();

        // 물리 화면 모드 변경 적용
        switch (currentModeIndex)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 1: Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break;
            case 2: Screen.fullScreenMode = FullScreenMode.Windowed; break;
        }
    }

    private void UpdateDisplayModeDisplay()
    {
        if (currentModeIndex >= 0 && currentModeIndex < modeOptions.Count)
        {
            modeValueText.text = modeOptions[currentModeIndex];
        }
    }

    // --- 감마 제어 로직 ---
    private void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Graphic_Brightness", value);
        if (brightnessText != null) brightnessText.text = $"{value:F1}";

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = (value - 1.0f) * 2f;
        }
    }
}