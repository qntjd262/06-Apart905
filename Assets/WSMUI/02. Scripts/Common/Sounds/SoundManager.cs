using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private SoundDataSO[] bgmDatas;
    [SerializeField] private SoundDataSO[] sfxDatas;

    [Header("Scene BGM Mapping")]
    [SerializeField] private SceneBGMData[] sceneBGMMap;

    private Dictionary<string, SoundDataSO> _bgmDict;
    private Dictionary<string, SoundDataSO> _sfxDict;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    protected override void Awake()
    {
        base.Awake();
        _bgmDict = bgmDatas.ToDictionary(x => x.key);
        _sfxDict = sfxDatas.ToDictionary(x => x.key);

        bgmSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        sfxSource.volume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }
    public void PlaySceneBGM(string sceneName)
    {
        SceneBGMData mapping = sceneBGMMap.FirstOrDefault(x => x.sceneType.ToString() == sceneName);
        if (mapping != null)
            PlayBGM(mapping.bgmKey);
        else
            StopBGM();
    }
    public void PlayBGM(string key)
    {
        if (!_bgmDict.TryGetValue(key, out SoundDataSO data)) return;
        if (bgmSource.clip == data.clip && bgmSource.isPlaying) return;

        bgmSource.clip = data.clip;
        bgmSource.pitch = data.pitch;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PauseBGM()
    {
        bgmSource.Pause(); // 현재 재생 위치를 기억하고 멈춤
    }

    public void ResumeBGM()
    {
        bgmSource.UnPause(); // 기억된 위치부터 다시 재생
    }
    public void PlaySFX(string key)
    {
        if (!_sfxDict.TryGetValue(key, out SoundDataSO data)) return;
        sfxSource.pitch = data.pitch;
        sfxSource.PlayOneShot(data.clip, data.volume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    protected override void OnSceneUnloaded(Scene scene) { }
}

[System.Serializable]
public class SceneBGMData
{
    public Constants.ESceneType sceneType;
    public string bgmKey;
}