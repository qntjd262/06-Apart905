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
        this.gameObject.SetActive(true);

        ConfigureSlider(masterSlider);
        ConfigureSlider(bgmSlider);
        ConfigureSlider(sfxSlider);

        float masterVol = PlayerPrefs.GetFloat("Volume_Master", 1f);
        float bgmVol = PlayerPrefs.GetFloat("Volume_BGM", 1f);
        float sfxVol = PlayerPrefs.GetFloat("Volume_SFX", 1f);

        masterSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        masterSlider.value = masterVol;
        bgmSlider.value = bgmVol;
        sfxSlider.value = sfxVol;

        UpdateVolumeText(masterText, masterVol);
        UpdateVolumeText(bgmText, bgmVol);
        UpdateVolumeText(sfxText, sfxVol);

        SetAudioMixerValue("Master", masterVol);
        SetAudioMixerValue("BGM", bgmVol);
        SetAudioMixerValue("SFX", sfxVol);

        masterSlider.onValueChanged.AddListener(value => OnVolumeSliderChanged("Master", value, masterText));
        bgmSlider.onValueChanged.AddListener(value => OnVolumeSliderChanged("BGM", value, bgmText));
        sfxSlider.onValueChanged.AddListener(value => OnVolumeSliderChanged("SFX", value, sfxText));

        isInitialized = true;
    }

    private void ConfigureSlider(Slider slider)
    {
        if (slider == null) return;
        slider.interactable = true;
        slider.minValue = 0.0001f;  
        slider.maxValue = 1f;       
    }

    private void OnVolumeSliderChanged(string parameterName, float value, TextMeshProUGUI textComponent)
    {
        // [핵심 수정] 실시간 PlayerPrefs 저장 기능 삭제. (드래그할 때는 믹서 볼륨만 조절됨)
        UpdateVolumeText(textComponent, value);
        SetAudioMixerValue(parameterName, value);
    }

    private void SetAudioMixerValue(string parameterName, float value)
    {
        if (audioMixer == null) return;
        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    private void UpdateVolumeText(TextMeshProUGUI textComponent, float value)
    {
        if (textComponent != null)
            textComponent.text = $"{(value * 100f):F0}%";
    }

    // [추가] '확인' 버튼을 눌렀을 때만 호출되는 저장 함수
    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("Volume_Master", masterSlider.value);
        PlayerPrefs.SetFloat("Volume_BGM", bgmSlider.value);
        PlayerPrefs.SetFloat("Volume_SFX", sfxSlider.value);
    }

    // [추가] '돌아가기' 버튼을 누르면 믹서와 UI를 이전 저장 상태로 롤백하는 함수
    public void RevertOptions()
    {
        Initialize(); 
    }
}