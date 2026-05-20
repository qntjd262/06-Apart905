using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeFlow : MonoBehaviour
{
    /* 시간의 흐름에 따라 Directional Light의 회전과 색깔이 바뀌는 스크립트
     * 실제 시간 1분 = 게임 시간 1시간
     * x축은 90도, y축은 0도일 때가 12시
     * 시간에 따라 Environment Reflection의 Intensity Multiplier도 변경
     */

    [Header("씬에 배치된 Directional Light 할당 필요")]
    [SerializeField] private GameObject directionalLight;

    [Header("현재 시간 (현실 1분 = 게임 1시간)")]
    [SerializeField] private int hours;
    [SerializeField] private float minutes;

    [Header("현재 시간 (현실 1분 = 게임 1시간)")]
    [SerializeField] private Color dayColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color nightColor = new Color(0.2f, 0.2f, 0.2f);

    private void LateUpdate()
    {
        minutes += Time.deltaTime;
        if (minutes >= 60f)
        {
            hours++;
            minutes = 0f;

            if (hours >= 24)
                hours = 0;
        }

        RotateLight();
        ChangeLightColor();
    }

    private void RotateLight()      // Directional Light 회전
    {
        float rotationXY = (hours + (minutes / 60f)) / 24f * 360;

        directionalLight.transform.rotation = Quaternion.Euler(rotationXY - 90f, rotationXY - 180f, 0f);
    }

    private void ChangeLightColor()     // Directional Light 색깔 변경
    {
        if (hours >= 5 && hours <= 8)
        {
            float t = (hours - 5) + (minutes / 60f);
            directionalLight.GetComponent<Light>().color = Color.Lerp(nightColor, dayColor, t / 3f);
            RenderSettings.reflectionIntensity = 0.2f + t / 10f;
        }
        else if (hours > 8 && hours < 16)
        {
            directionalLight.GetComponent<Light>().color = dayColor;
            RenderSettings.reflectionIntensity = 0.5f;
        }   
        else if (hours >= 16 && hours <= 20)
        {
           float t = (hours - 16) + (minutes / 60f);
            directionalLight.GetComponent<Light>().color = Color.Lerp(dayColor, nightColor, t / 4f);
            RenderSettings.reflectionIntensity = 0.5f - t / 11f;
        }
        else
        {
            directionalLight.GetComponent<Light>().color = nightColor;
            RenderSettings.reflectionIntensity = 0.15f;
        }            
    }
}
