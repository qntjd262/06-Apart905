using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SoundOptions : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; 

    [Header("마스터 볼륨")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI masterText;

    [Header("BGM 볼륨")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmText;

    [Header("SFX 볼륨")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;

    private bool isInitialized = false;

    public void Initialize()
    {
        // 중복 초기화 방지 및 슬라이더 컴포넌트 강제 리프레시
        this.gameObject.SetActive(true);

        // [방어 코드] 슬라이더 컴포넌트 기본 세팅 강제 주입
        ConfigureSlider(masterSlider);
        ConfigureSlider(bgmSlider);
        ConfigureSlider(sfxSlider);

        // 1. 데이터 로드 (기본값 1f = 100%)
        float masterVol = PlayerPrefs.GetFloat("Volume_Master", 1f);
        float bgmVol = PlayerPrefs.GetFloat("Volume_BGM", 1f);
        float sfxVol = PlayerPrefs.GetFloat("Volume_SFX", 1f);

        // 2. 리스너 중복 등록 방지를 위해 기존 연결 제거 후 재등록
        masterSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        // 3. UI 슬라이더 값 주입 (값이 꺾여있던 슬라이더가 정상 위치로 쫙 이동함)
        masterSlider.value = masterVol;
        bgmSlider.value = bgmVol;
        sfxSlider.value = sfxVol;

        // 4. 텍스트 출력 최신화
        UpdateVolumeText(masterText, masterVol);
        UpdateVolumeText(bgmText, bgmVol);
        UpdateVolumeText(sfxText, sfxVol);

        // 5. 오디오 믹서 물리 값 동기화
        SetAudioMixerValue("Master", masterVol);
        SetAudioMixerValue("BGM", bgmVol);
        SetAudioMixerValue("SFX", sfxVol);

        // 6. 실시간 조절을 위한 리스너 최종 바인딩
        masterSlider.onValueChanged.AddListener(value => OnVolumeSliderChanged("Master", value, masterText));
        bgmSlider.onValueChanged.AddListener(value => OnVolumeSliderChanged("BGM", value, bgmText));
        sfxSlider.onValueChanged.AddListener(value => OnVolumeSliderChanged("SFX", value, sfxText));

        isInitialized = true;
    }

    private void ConfigureSlider(Slider slider)
    {
        if (slider == null) return;
        slider.interactable = true; // 잠금 해제
        slider.minValue = 0.0001f;  // 볼륨 오차 방지 최소값
        slider.maxValue = 1f;       // 최대값
    }

    private void OnVolumeSliderChanged(string parameterName, float value, TextMeshProUGUI textComponent)
    {
        PlayerPrefs.SetFloat($"Volume_{parameterName}", value);
        UpdateVolumeText(textComponent, value);
        SetAudioMixerValue(parameterName, value);
    }

    private void SetAudioMixerValue(string parameterName, float value)
    {
        if (audioMixer == null) return;
        
        // 로그 스케일 연산으로 오디오 믹서 데시벨 제어
        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    private void UpdateVolumeText(TextMeshProUGUI textComponent, float value)
    {
        if (textComponent != null)
            textComponent.text = $"{(value * 100f):F0}%";
    }
}