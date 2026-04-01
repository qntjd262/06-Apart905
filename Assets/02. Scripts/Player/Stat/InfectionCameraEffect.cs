using System.Collections;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
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
        Noise,
        Shake
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

            yield return RandomEffectCoroutine();

            float waitInterval = 15f;
            if(currentStage == 2) waitInterval = 10f;
            else if(currentStage == 3) waitInterval = 5f;

            yield return new WaitForSeconds(waitInterval);
        }
    }

    //카메라 효과 왜곡, 노이즈 둘 중 하나 랜덤 재생
    private IEnumerator RandomEffectCoroutine()
    {
        currentEffectType selectedEffect = (currentEffectType)Random.Range(0,3);

        switch (selectedEffect)
        {
            case currentEffectType.Distortion:
                yield return DistortionCoroutine();
                break;
            case currentEffectType.Noise:
                yield return NoiseCoroutine();
                break;
            case currentEffectType.Shake:
                yield return ShakeCoroutine();
                break;
        }

        ResetEffects();
    }

    #region 카메라 효과 재생 코루틴
    private IEnumerator DistortionCoroutine()
    {
        float intensityValue = GetCurrentIntensity();

        if(lensDistortion != null)
        {
            Debug.Log("왜곡 실행");
            DOTween.To(()=> lensDistortion.intensity.value,x => lensDistortion.intensity.value = x, -1f*intensityValue, 1f).SetId("CameraFX");
            DOTween.To(() => lensDistortion.scale.value, x => lensDistortion.scale.value = x, 1f + (0.5f * intensityValue), 1f).SetId("CameraFX");
                
            yield return new WaitForSeconds(5f);
                    
            DOTween.To(()=> lensDistortion.intensity.value,x => lensDistortion.intensity.value = x, 0f, 2f).SetId("CameraFX");
            DOTween.To(() => lensDistortion.scale.value, x => lensDistortion.scale.value = x, 1f, 2f).SetId("CameraFX");

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator NoiseCoroutine()
    {
        float intensityValue = GetCurrentIntensity();

        if(filmGrain != null)
        {
            Debug.Log("Noise 실행");
            DOTween.To(() => filmGrain.intensity.value, x => filmGrain.intensity.value = x, 1f * intensityValue, 1f).SetId("CameraFX");
            DOTween.To(() => filmGrain.response.value, x => filmGrain.response.value = x, 0.8f - (0.8f * intensityValue), 1f).SetId("CameraFX");

            yield return new WaitForSeconds(5f);

            DOTween.To(() => filmGrain.intensity.value, x => filmGrain.intensity.value = x, 0f, 2f).SetId("CameraFX");
            DOTween.To(() => filmGrain.response.value, x => filmGrain.response.value = x, 0.8f, 2f).SetId("CameraFX");

            yield return new WaitForSeconds(2f);
        }
         
    }

    private IEnumerator ShakeCoroutine()
    {
        Debug.Log("Shake 실행");

        float intensityValue = GetCurrentIntensity();

        
        float shakeDuration = 3f;

        float currentShakeStrength = 0.05f + (intensityValue * 0.45f);
        int currentVibrato = Mathf.RoundToInt(10f+intensityValue*40f);

        transform.DOShakePosition(
            duration : shakeDuration,
            strength : currentShakeStrength,
            vibrato : currentVibrato,
            randomness : 90f,
            fadeOut : true
        ).SetId("CameraShakeFX");

        yield return new WaitForSeconds(shakeDuration);
    }


    #endregion
    
    private float GetCurrentIntensity()
    {
        float currentInfection = playerStat.infection.currentValue;
        return Mathf.Clamp01(0.5f + (currentInfection/100f) * 0.5f);
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
