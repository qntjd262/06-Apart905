using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGamemanager : MonoBehaviour
{
    public enum DayState
    {
        Day, Night
    }
    private DayState _currDayState;

    public static PlayerGamemanager Instance;

    
    public static event Action OnGameStatChangeTime;
    // 몬스터에게 낮 밤 변경을 알리는 Action
    public static event Action<DayState> OnDayStateChange;

    [Header("시간 설정")]
    private float timer = 0f;
    private int dayCount = 0;
    private int hours = 0;
    private int minutes = 0;
    private float seconds = 0f;

    [Header("시간 배속")]
    //시간 배속 조절 변수
    public float timeMultiplier = 60f;
    

    [Header("스탯 감소 인터벌")]
    //게임 시간 스탯 인터벌 (분 단위)
    private int statDecreaseMinuteInterval = 3;
    private int minutePassCount = 0;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //현재 배고픔, 감염도, 갈증이 맥스에 달할 때마다 인터벌 후 hp감소도 적용
    void Update()
    {
        CalculateTime();
        CalculateDayState();
    }

    private void CalculateTime()
    {
        seconds += Time.deltaTime * timeMultiplier;

        if(seconds >= 60f)
        {
            minutes ++;
            seconds = 0f;
            minutePassCount ++;

            if(minutePassCount >= statDecreaseMinuteInterval)
            {
                OnGameStatChangeTime?.Invoke();
                minutePassCount = 0;
            }
            if(minutes >= 60)
            {
                hours++;
                minutes = 0;

                if(hours >= 24)
                {
                    dayCount ++;
                    hours = 0;
                }
            }
        }
    }

    //게임 시간을 UI상으로 표시하기 위한 함수
    public string GetFormattedTime()
    {
        return string.Format("{0}일 {1:00} : {2:00}", dayCount,hours,minutes);
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene(2);
    }

    // 낮밤 계산용 함수
    private void CalculateDayState()
    {
        // 6시부터 18시 사이까지를 낮으로 판정
        if (hours > 6 && hours < 18)
        {
            _currDayState = DayState.Day;
        }
        else
        {
            _currDayState = DayState.Night;
        }
  
        OnDayStateChange?.Invoke(_currDayState);
    }
}
