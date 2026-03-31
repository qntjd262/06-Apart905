using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InfectionCameraEffect : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private Volume postProcessVolume;

    private LensDistortion lensDistortion;
    private FilmGrain filmGrain;
    private Coroutine currentLoopCoroutine;

    private enum currentEffectType
    {
        Distortion,
        Noise
    }

    void Start()
    {
        //초기화
        if(postProcessVolume.profile.TryGet(out lensDistortion) && postProcessVolume.profile.TryGet(out filmGrain))
        {
            ResetEffects();
        }
    }

    void OnEnable()
    {
        playerStat.OnInfectionStateBool += HandleInfectionState;
    }

    void OnDisable()
    {
        playerStat.OnInfectionStateBool -= HandleInfectionState;
    }

    //PlayerStat에서 감염이 30이상일 때 카메라 효과 시작, 30미만일때 종료 메서드
    private void HandleInfectionState(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("카메라 효과 시작");
            if(currentLoopCoroutine == null)
            {
                StartCoroutine(PlayeEffectCoroutine());
            }
        }
        else
        {
            Debug.Log("카메라 효과 종료");
            if(currentLoopCoroutine != null)
            {
                StopCoroutine(currentLoopCoroutine);
                currentLoopCoroutine = null;
            }
            ResetEffects();
        }
    }

    //감염 단계에 따른 효과 재생 주기 조절
    private IEnumerator PlayeEffectCoroutine()
    {
        while (true)
        {
            int currentStage = playerStat.currentInfectionStage;

            yield return StartCoroutine(RandomEffectCoroutine());

            float waitInterval = 15f;
            if(currentStage == 2) waitInterval = 10f;
            else if(currentStage == 3) waitInterval = 5f;

            yield return new WaitForSeconds(waitInterval);
        }
    }

    //카메라 효과 왜곡, 노이즈 둘 중 하나 랜덤 재생
    private IEnumerator RandomEffectCoroutine()
    {
        float currentInfection = playerStat.infection.currentValue;
        float intensityValue = Mathf.Clamp01(0.5f + (currentInfection/100f) * 0.5f);

        currentEffectType selectedEffect = (currentEffectType)Random.Range(0,2);
        int duration = Random.Range(4, 6);

        switch (selectedEffect)
        {
            case currentEffectType.Distortion:
                if(lensDistortion != null)
                {
                    lensDistortion.intensity.value = -1f * intensityValue;
                    lensDistortion.scale.value = 1.5f;
                }
                break;
            case currentEffectType.Noise:
                if(filmGrain != null)
                {
                    filmGrain.intensity.value = 1.0f;
                    filmGrain.response.value = 0f;
                }
                break;
        }

        yield return new WaitForSeconds(duration);
        ResetEffects();
    }

    private void ResetEffects()
    {
        if(lensDistortion != null)
        {
            lensDistortion.intensity.value = 0f;
            lensDistortion.scale.value = 1f;
        }

        if(filmGrain != null)
        {
            filmGrain.intensity.value = 0f;
            filmGrain.response.value = 0.8f;
        }
    }

}
