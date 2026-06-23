using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeFlow : MonoBehaviour
{
    /* 인게임 시간이 흐름에 따라 Directional Light의 회전을 바꾸는 스크립트
     * 현실 시간 1분 = 게임 시간 1시간
     * X축 90도, Y축 0도 일 때가 낮 12시 정오
     * 시간에 따라 Environment Reflection의 Intensity Multiplier 값을 동적으로 변경
     */
    public static TimeFlow Instance {get; private set;}

    [Header("진행된 날짜")]
    [SerializeField] private int days = 1;
    public int Days => days;

    public static Action<int> OnDayChanged;

    [Header("낮과 밤을 표현할 Directional Light 오브젝트 할당")]
    [SerializeField] private GameObject directionalLight;

    [Header("게임 내 현재 시간 설정 (현실 1분 = 게임 1시간)")]
    [SerializeField] private int hours;
    [SerializeField] private float minutes;

    [Header("시간대별 하늘과 빛의 색상 설정")]
    [SerializeField] private Color dayColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color nightColor = new Color(0.2f, 0.2f, 0.2f);

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        minutes += Time.deltaTime;
        if (minutes >= 60f)
        {
            hours++;
            minutes = 0f;

            if (hours >= 24)
            {
                hours = 0;
                days++;
                
                OnDayChanged?.Invoke(days);

                Debug.Log($"게임 속 하루가 지나 날짜가 변경되었습니다! 현재: {days}일차");
            }
        }

        RotateLight();
        ChangeLightColor();
    }

    private void RotateLight()      // Directional Light ȸ��
    {
        float rotationXY = (hours + (minutes / 60f)) / 24f * 360;

        directionalLight.transform.rotation = Quaternion.Euler(rotationXY - 90f, rotationXY - 180f, 0f);
    }

    private void ChangeLightColor()     // Directional Light ���� ����
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
