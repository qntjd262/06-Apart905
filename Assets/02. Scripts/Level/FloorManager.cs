using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    /* 게임 실행으로 Main씬에서 Game씬으로 넘어갈 때, 1층부터 17층까지의 각 34 세대에 랜덤으로 가구 배치를 적용시키는 코드
     * 랜덤으로 프리셋을 적용시키는 것 외에, 고정 오브젝트(퀘스트 아이템)의 위치는 매 게임 동일하게 적용
     * 
     * 플레이어가 해당 층을 벗어나면 그 위치에 그 층의 가구들은 비활성화(SetActive false)
     * 
     * 현재 구현된 가구 프리셋 -> 2개
     * 
     * 손전등(905호 거실 탁자 위), 부품(203호 방2), 부서진 라디오(605호 주방 베란다), 배터리 팩(1205호 방2), 열쇠(1403호 방3), 휘발유(1705호 방3 베란다)
     * 친한 형(803호 방3), 이상한 여자(305호 방1), 충격먹은 청년(1405호 방3), 좀비가 된 경비원(1403호 복도(거실과 주방 사이))
     */


    [Header("가구배치 프리셋")]
    [SerializeField] private List<GameObject> Presets = new List<GameObject>();               // 옥상 제외 17층, 각 층에 2세대 -> 총 34가구에 적용시킬 가구 배치 프리셋
    [SerializeField] private GameObject Presets_Ingame;

    [Header("각 층의 좌/우측 방 위치")]
    [SerializeField] private Transform[] Rooms = new Transform[6];                 // 이동하는 3개의 층의 각 방 위치

    [Header("**퀘스트 아이템**")]
    [SerializeField] private GameObject[] QuestItems = new GameObject[6];           // 사용되는 퀘스트 아이템들(손전등, 부품, 부서진 라디오, 배터리 팩, 열쇠, 휘발유)

    [Header("**퀘스트 NPC**")]
    [SerializeField] private GameObject[] QuestNPCs = new GameObject[4];           // 고정 출현 퀘스트 NPC들(친한 형, 이상한 여자, 충격먹은 청년, 좀비가 된 경비원)

    private MoveLevel moveLevel;            // MoveLevel의 _currLevel을 받기위함 
    private Vector3 Floor9_leftpos;
    private Vector3 Floor9_rightpos;


    private void Awake()
    {
        moveLevel = FindFirstObjectByType<MoveLevel>();

        InitialSetting();
    }

    private void InitialSetting()                               // 게임 시작 시 실행할 함수 모음
    {
        AllocatePresets();
        RandomShuffle(Presets);

        Floor9_leftpos = moveLevel.LevelMid.transform.Find("Room_Left").transform.position;
        Floor9_rightpos = moveLevel.LevelMid.transform.Find("Room_Right").transform.position;
    }

    private void AllocatePresets()                              // 셔플할 인 게임상의 가구 프리셋들 할당
    {
        for(int i = 0; i < 34; i++)
        {
            Presets[i] = Presets_Ingame.transform.GetChild(i).gameObject;
        }
    }

    private void RandomShuffle(List<GameObject> list)             // 게임 시작할 때 프리셋의 순서를 랜덤으로 셔플
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private Vector3 RoomPos(int num, bool isLeft)               // 층과 방향을 입력받으면 해당 위치를 return하는 함수
    {
        if(num > 9)
        {
            if (isLeft)
            {
                float yPos = Floor9_leftpos.y + (5.4f * (num - 9));
                return new Vector3(Floor9_leftpos.x, yPos, Floor9_leftpos.z);
        }
            else
            {
                float yPos = Floor9_rightpos.y + (5.4f * (num - 9));
                return new Vector3(Floor9_rightpos.x, yPos, Floor9_rightpos.z);
            }
        }
        else if(num < 9)
        {
            if (isLeft)
            {
                float yPos = Floor9_leftpos.y - (5.4f * (9 - num));
                return new Vector3(Floor9_leftpos.x, yPos, Floor9_leftpos.z);
            }
            else
            {
                float yPos = Floor9_rightpos.y - (5.4f * (9 - num));
                return new Vector3(Floor9_rightpos.x, yPos, Floor9_rightpos.z);
            }
        }
        else
        {
            if (isLeft)
                return Floor9_leftpos;
            else
                return Floor9_rightpos;
        }
    }

    private void SetFurnitures()                                // 각 층에 가구를 배치하는 함수
    {
        int x = 0;
        int floor = 1;

        while (x < 34)
        {
            Presets[x++].transform.position = RoomPos(floor, true);             // 좌측방
            Presets[x++].transform.position = RoomPos(floor++, false);            // 우측방
        }
    }
}
