using System;
using UnityEditor.PackageManager.UI;
using UnityEngine;


public class MicNoiseDetector : MonoBehaviour
{
    [SerializeField] private PlayerNoise playerNoise;

    [Header("마이크 설정")]
    
    [SerializeField] private float threshold = 0.01f;
    [SerializeField] private float maxMicVolume = 0.08f;
    [SerializeField] private float micSensitivity = 10f;

    [Header("소음 반경 설정")]
    [SerializeField] private float maxNoiseRadius = 20f;

    [Header("스무딩 설정")]
    [SerializeField] private float smoothingSpeed = 10f;
    [SerializeField] private float riseMultiplier = 2f;

    [Header("인터벌 설정")]
    [SerializeField] private float rippleInterval = 1f;
    private float peakVolumeInInterval = 0f;

    [Header("디버그 설정")]
    [SerializeField] private bool showDebugLog = false;

    [Header("실시간 볼륨 레벨")]
    [SerializeField] private float volumeLevel;

    private float nextRippleTime = 0f;
    private float smoothedVolume = 0f;
    private AudioClip micClip;
    private string micName;
    private int sampleWindow = 1024;

    void Start()
    {
        if(Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 10, 44100);
            Debug.Log($"마이크 켜짐 : {micName}");
        }
        else
        {
            Debug.Log("마이크 없음");
        }
    }

    void Update()
    {
        if(micClip == null) return;

        float rawVolume = GetVolumRMS()* micSensitivity;

        float targetSmoothing = rawVolume > smoothedVolume ? smoothingSpeed * riseMultiplier : smoothingSpeed;

        smoothedVolume = Mathf.Lerp(smoothedVolume, rawVolume, targetSmoothing * Time.deltaTime);

        volumeLevel = Mathf.Clamp((smoothedVolume / maxMicVolume) * 100f, 0f, 100f);

        if(volumeLevel > peakVolumeInInterval)
        {
            peakVolumeInInterval = MathF.Min(volumeLevel, maxNoiseRadius);
        }

        if(smoothedVolume > threshold && Time.time >= nextRippleTime)
        {

            if(playerNoise != null)
            {
                playerNoise.TriggerOneShotNoise(peakVolumeInInterval);
            }

            if (showDebugLog)
            {
                Debug.Log($"소음 감지 최종 반경 : {volumeLevel:F1}, 적용된 피크 반경 {peakVolumeInInterval:F1}");
            }

            nextRippleTime = Time.time + rippleInterval;
            peakVolumeInInterval = 0f;
        }
    }

    private float GetVolumRMS()
    {
        int micPosition = Microphone.GetPosition(micName);
        if(micPosition < sampleWindow) return 0f;

        float[] waveData = new float[sampleWindow];

        micClip.GetData(waveData, micPosition - sampleWindow);

        float sumSquares = 0f;
        for(int i = 0 ; i < sampleWindow; i++)
        {
            sumSquares += waveData[i] * waveData[i];
        }
        return Mathf.Sqrt(sumSquares / sampleWindow);
    }
}
