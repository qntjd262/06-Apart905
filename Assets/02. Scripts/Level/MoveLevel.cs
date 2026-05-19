    using UnityEngine;

public class MoveLevel : MonoBehaviour
{
    /*    층을 이동할 때 작동하는 스크립트
     *    두 층 차이나는 층은 사라지고 위/아래로 이동되는 방식
     *    이동될 때 해당 층에 있던 오브젝트/몬스터/NPC 등은 비활성화되고 그 자리에 남아있음
     *    계단 중앙에 가면 스크립트가 작동하는 방식
     *    Player스크립트에서 Player의 상태에 따라 위/아래를 구분     
     *    플레이어 외의 요소들이 이동하는것 보단 플레이어가 이동하고 층만 따라가는게 더 효율적이라고 생각함
     *    층 간격은 prefab기준으로 5.4
     *    바닥 y축은 -45.9
     */
    
    public enum PlayerState         // 플레이어의 층 이동 상태
    {
        None, MoveUp, MoveDown
    }
    public PlayerState state;
    public bool check = false;


    [SerializeField]
    private GameObject
        _LevelBott, _LevelMid, _LevelTop,  // 이동시킬 3개의 층들
        _RoofTop, _Ground;                          // 옥상, 1층 바닥
    [SerializeField] private int _currLevel;         // 현재 층 수
    public int CurrLevel => _currLevel;
    public GameObject LevelBottom { get{ return _LevelBott; }}
    public GameObject LevelMid { get{ return _LevelMid; }}
    public GameObject LevelTop { get{ return _LevelTop; }}


    private void Start()
    {
        _currLevel = 9;             // 시작할 때 9층에서 시작
    }

    public void MoveUp()
    {
        if (_currLevel > 2 && _currLevel < 16)          // 18층은 옥상 구현
        {          
            _LevelBott.transform.Translate(0, 5.4f * 3, 0);   // 최하층을 제일 위로
            IsMoveUp(true);

            // 층 이동에 따른 오브젝트/몬스터/NPC 비활성화 및 위치 조정 코드 작성
        }
        else if (_currLevel == 3)
        {
            _Ground.SetActive(false);
        }
        else
        {
            _RoofTop.transform.position = new Vector3(0, _LevelTop.transform.position.y + 5.4f, 0);    // 옥상으로 이동
            _RoofTop.SetActive(true);
        }

        _currLevel++;
    }

    public void MoveDown()
    {
        if (_currLevel > 2 && _currLevel < 17)           // 최상층을 제일 아래로
        {
            _LevelTop.transform.Translate(0, -5.4f * 3, 0);
            IsMoveUp(false);

            if (_currLevel == 16)                       // 옥상 비활성화
            {
                _RoofTop.SetActive(false);
            }
            // 층 이동에 따른 오브젝트/몬스터/NPC 비활성화 및 위치 조정 코드 작성
        }
        else if( _currLevel == 2)
        {
            _Ground.SetActive(true);
        }

            _currLevel--;                               // 층 감소
    }

    private void IsMoveUp(bool isMoveUp)     // 층 이동 후 층 위치 조정
    {
        if (isMoveUp)           // 1-2-3 -> 2-3-1
        {
            GameObject LvTop = _LevelBott;   // 현재 최상층은 levelbottom
            GameObject LvMiddle = _LevelTop; // 현재 중간층은 leveltop

            _LevelBott = _LevelMid;         // 최하층을 2로
            _LevelMid = LvMiddle;           // 중간층을 3으로
            _LevelTop = LvTop;              // 최상층을 1로
        }
        else                    // 1-2-3 -> 3-1-2
        {
            GameObject LvBottom = _LevelTop;   // 현재 최하층은 leveltop
            GameObject LvMiddle = _LevelBott; // 현재 중간층은 levelBottom

            _LevelTop = _LevelMid;         // 최상층을 2로
            _LevelMid = LvMiddle;          // 중간층을 1로
            _LevelBott = LvBottom;         // 최하층을 3으로            
        }
    }
}
