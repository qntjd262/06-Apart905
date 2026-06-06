using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    /* Game 씬으로 넘어갈 때, 1층부터 17층까지의 각 34 세대에 랜덤으로 가구 배치를 적용시키는 코드
     * 랜덤으로 프리셋을 적용시키는 것 외에, 고정 오브젝트(퀘스트 아이템)의 위치는 매 게임 동일하게 적용
     * 
     * 플레이어가 해당 층을 벗어나면 그 위치에 그 층의 가구들은 비활성화(SetActive false)
     * 
     * 현재 구현된 가구 프리셋 -> 2개
     * 
     * 손전등(905호 거실 탁자 위), 부품(203호 방2), 부서진 라디오(605호 주방 베란다), 배터리 팩(1205호 방2), 열쇠(1403호 방3), 휘발유(1705호 방3 베란다)
     * 친한 형(803호 방3), 이상한 여자(305호 방1), 충격먹은 청년(1405호 방3), 좀비가 된 경비원(1403호 복도(거실과 주방 사이))
     * 
     */

    [SerializeField] private MoveLevel _moveLevel;                                  // MoveLevel의 _currLevel을 받기위함 

    [Header("가구배치 프리셋")]
    [SerializeField] private List<GameObject> _Presets = new List<GameObject>();    // 옥상 제외 17층, 각 층에 2세대 -> 총 34가구에 적용시킬 가구 배치 프리셋
    [SerializeField] private GameObject _Presets_Ingame;

    [Header("**퀘스트 아이템**")]
    [SerializeField] private GameObject[] _QuestItems = new GameObject[6];          // 사용되는 퀘스트 아이템들(손전등, 부품, 부서진 라디오, 배터리 팩, 열쇠, 휘발유)

    [Header("**퀘스트 NPC**")]
    [SerializeField] private GameObject[] _QuestNPCs = new GameObject[4];           // 고정 출현 퀘스트 NPC들(친한 형, 이상한 여자, 충격먹은 청년, 좀비가 된 경비원)

    [Header("**각 층에 배치된 좀비 스포너들**")]
    [SerializeField] private GameObject[] _ZombieSpawners = new GameObject[8];      // 2, 3, 6, 8, 9, 12, 14, 17 층의 스포너


    private Vector3 Floor9_leftpos;
    private Vector3 Floor9_rightpos;


    private void Awake()
    {
        InitialSetting();
    }

    /* 게임 첫 시작 시 실행될 함수
     * 불러오기로 실행은 불가
     * 
     * 1. 씬 시작 시, 각 방의 위치를 잡기 위한 초기값 설정(플레이어가 위치하는 9층 기준)
     * 2. AllocatePresets: 미리 Active false상태로 배치된 가구 프리셋들을 셔플하기 위해 Presets 리스트에 할당
     * 3. RandomShuffle(Presets): Fisher-Yates 셔플, 뒤에서부터 앞으로 순회하며 현재 인덱스 이하의 랜덤 위치와 스왑하는 방식으로 매개변수 리스트를 셔플
     * 4. SetFurnitures: 셔플된 Presets 리스트 안의 가구 프리셋들을 1층부터 좌-우순서로 17층 우측방까지 배치
     * 5. SetActiveFurnitures(currLevel, isUp, isInit): 플레이어 기준 위/아래/중간 층의 가구만 활성화시키는 함수
     * 5. SetInitialQuests: 모든 퀘스트 아이템과 NPC를 정해진 위치에 배치하는 함수
     */
    private void InitialSetting()                               
    {
        Floor9_leftpos = _moveLevel.LevelMid.transform.Find("Room_Left").transform.position;
        Floor9_rightpos = _moveLevel.LevelMid.transform.Find("Room_Right").transform.position;

        AllocatePresets();
        RandomShuffle(_Presets);
        SetFurnitures();
        SetActiveFurnitures(_moveLevel.CurrLevel, false, true);
        SetQuests(_moveLevel.CurrLevel);
        SetZombies(_moveLevel.CurrLevel);
    }

    private void AllocatePresets()                              // 셔플할 인 게임상의 가구 프리셋들 할당
    {
        for(int i = 0; i < 34; i++)
        {
            _Presets[i] = _Presets_Ingame.transform.GetChild(i).gameObject;
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
        int x = 0;                                              // x는 프리셋 카운트용, 프리셋 위치를 할당할 때 마다 카운트 후위 덧셈 
        int floor = 1;                                          // floor는 층 수, 각 층의 좌/우측방에 할당이 되면 카운트 후위 덧셈

        while (x < 34)
        {
            _Presets[x].transform.localScale = new Vector3(-2, 2, 2);
            _Presets[x++].transform.position = RoomPos(floor, true);               // 좌측방

            _Presets[x].transform.localScale = new Vector3(2, 2, 2);
            _Presets[x++].transform.position = RoomPos(floor++, false);            // 우측방
        }
    }
    
    /* MoveLevel에서 호출될 함수, 층 이동 시 특정 층 가구의 활성/비활성화를 담당
     * _currLevel을 받아 해당 층과 그 위/아래 층의 가구만 활성, 그 외는 비활성화
     */
    public void SetActiveFurnitures(int _currLevel, bool isUp, bool isInit)                             
    {
        int presetCnt = (2 * _currLevel - 2);                           // 각 층당 방 2개가 있으니 각 층의 좌측방 preset의 순서는 2n-2로 적용하면 현재 층의 프리셋 넘버링을 구할 수 있음

        // 새 게임, 불러오기 등 Game 씬으로 넘어왔을 때만 실행
        // 플레이어가 예외상황(1층에 있거나 옥상층이 활성화된 상황)을 제외하고는 위/현재/아래 3개층의 6개 방의 가구를 활성화
        if (isInit)                                                     // 새 게임 혹은 불러오기 시 호출
        {
            if(_currLevel == 1)                                         // 플레이어가 1층에 있을 때, 1/2/3층의 가구를 활성화
            {
                int i = 0;                                              // 아래층이 없어 현재층부터 구현
                while (i < 6)
                {
                    _Presets[presetCnt + i].SetActive(true);             // presetCnt -> 0,1,2,3,4,5         
                    i++;
                }

            }
            else if((_currLevel == 17) || (_currLevel == 18))           // 플레이어가 17/18층에 있을 때, 16층과 맵 활성화 구조가 동일하기 때문에 16층과 동일하게 취급
            {
                _currLevel = 16;
                int i = -2;

                while (i < 4)
                {
                    _Presets[(presetCnt + i)].SetActive(true);
                    i++;
                }
            }
            else                                                        // 그 외에는 위/현재/아래 층의 가구 구현
            {
                int i = -2;

                while (i < 4)
                {
                    _Presets[(presetCnt + i)].SetActive(true);
                    i++;
                }
            }
        }

        // 게임 진행 중 층 이동 시 호출
        else if (isUp)                                                   // 위층으로 이동 시
        {
            int i = -2;

            while (i < 6)
            {
                if(i < 0)
                    _Presets[(presetCnt + i)].SetActive(false);
                else
                    _Presets[(presetCnt + i)].SetActive(true);

                i++;
            }
        }
        else                                                             // 아래층으로 이동 시
        {
            int i = 3;

            while (i > -5)
            {
                if (i > 1)
                    _Presets[(presetCnt + i)].SetActive(false);
                else
                    _Presets[(presetCnt + i)].SetActive(true);

                i--;
            }
        }
    }

    public void SetQuests(int _currLevel)
    {
        foreach (var qNPCs in _QuestNPCs)
            qNPCs.SetActive(false);

        switch (_currLevel) 
        {
            case 3:
                _QuestNPCs[1].SetActive(true);
                break;

            case 8:
                _QuestNPCs[0].SetActive(true);
                break;

            case 14:
                _QuestNPCs[2].SetActive(true);
                _QuestNPCs[3].SetActive(true);
                break;
        }    
    }
    public void SetZombies(int _currLevel)
    {
        foreach (var zSpawners in _ZombieSpawners)
            zSpawners.SetActive(false);

        // 현재 층 포함 위아래 층까지의 스포너를 활성화
        for (int i = -2; i <= 0; i++)
        {
            int targetLevel = _currLevel + i;
            if (targetLevel >= 0 && targetLevel < _ZombieSpawners.Length)
            {
                _ZombieSpawners[targetLevel].SetActive(true);
            }
        }


        //switch (_currLevel) // 2, 3, 6, 8, 9, 12, 14, 17
        //{
        //    case 2:
        //        _ZombieSpawners[0].SetActive(true);
        //        break;
        //    case 3:
        //        _ZombieSpawners[1].SetActive(true);
        //        break;
        //    case 6:
        //        _ZombieSpawners[2].SetActive(true);
        //        break;
        //    case 8:
        //        _ZombieSpawners[3].SetActive(true);
        //        break;
        //    case 9:
        //        _ZombieSpawners[4].SetActive(true);
        //        break;
        //    case 12:
        //        _ZombieSpawners[5].SetActive(true);
        //        break;
        //    case 14:
        //        _ZombieSpawners[6].SetActive(true);
        //        break;
        //    case 17:
        //        _ZombieSpawners[7].SetActive(true);
        //        break;
        //}    
    }
    public void MonsterFloorMove(MonsterController targetMonster, bool isUp)
    {
        int targetDir = isUp ? 1 : -1;
        int targetFloor = targetMonster.Spawner.CurrFloor + targetDir - 1;
        targetMonster.Spawner.transform.SetParent(_ZombieSpawners[targetFloor].transform);
        targetMonster.Spawner.CurrFloor = targetFloor + 1;
    }

}
