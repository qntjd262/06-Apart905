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
    public static TimeFlow Instance { get; private set; }

    [Header("진행된 날짜")]
    [SerializeField] private int days = 1;
    public int Days => days;

    public static Action<int> OnDayChanged;

    //ui용 추가
    public static Action<int, int, int> OnTimeChanged;

    [Header("낮과 밤을 표현할 Directional Light 오브젝트 할당")]
    [SerializeField] private GameObject directionalLight;

    [Header("게임 내 현재 시간 설정 (현실 1분 = 게임 1시간)")]
    [SerializeField] private int hours;
    [SerializeField] private float minutes;

    //ui용 추가
    private int _lastNotifiedMinute = -1;
    public int Hours => hours;
    public int MinutesInt => Mathf.FloorToInt(minutes);


    [Header("시간대별 하늘과 빛의 색상 설정")]
    [SerializeField] private Color dayColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color nightColor = new Color(0.2f, 0.2f, 0.2f);

    /* 불러오기 오류로 비활성화
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
    }*/

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
        //ui용 추가
        int currentMinuteInt = Mathf.FloorToInt(minutes);
        if (currentMinuteInt != _lastNotifiedMinute)
        {
            _lastNotifiedMinute = currentMinuteInt;
            OnTimeChanged?.Invoke(days, hours, currentMinuteInt);
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
        if (hours >= 5 && hours <= 9)
        {
            float t = (hours - 5) + (minutes / 60f);
            directionalLight.GetComponent<Light>().color = Color.Lerp(nightColor, dayColor, t / 3f);
            directionalLight.GetComponent<Light>().color = Color.Lerp(nightColor, dayColor, t / 4f);
            RenderSettings.reflectionIntensity = 0.2f + t / 10f;
        }
        else if (hours > 9 && hours < 17)
        {
            directionalLight.GetComponent<Light>().color = dayColor;
            RenderSettings.reflectionIntensity = 0.66f;
        }
        else if (hours >= 17 && hours <= 21)
        {
            float t = (hours - 17) + (minutes / 60f);
            directionalLight.GetComponent<Light>().color = Color.Lerp(dayColor, nightColor, t / 4f);
            RenderSettings.reflectionIntensity = 0.6f - t / 9.5f;
        }
        else
        {
            directionalLight.GetComponent<Light>().color = nightColor;
            RenderSettings.reflectionIntensity = 0.15f;
        }
    }
}
