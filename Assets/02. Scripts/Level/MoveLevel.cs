using UnityEngine;

public class MoveLevel : MonoBehaviour
{
    /*    층을 이동할 때 작동하는 스크립트
     *    두 층 차이나는 층은 사라지고 위/아래로 이동되는 방식
     *    이동될 때 해당 층에 있던 오브젝트/몬스터/NPC 등은 비활성화되고 그 자리에 남아있음
     *    계단 중앙에 가면 스크립트가 작동하는 방식
     *    Player스크립트에서 Player의 상태에 따라 위/아래를 구분     
     *    플레이어 외의 요소들이 이동하는것 보단 플레이어가 이동하고 층만 따라가는게 더 효율적이라고 생각함
     *    */
    
    public enum PlayerState         // 임시로 지정
    {
        MoveUp, MoveDown
    }
    public PlayerState state;       // 임시로 지정
    public Vector3 size;


    private GameObject
        _Level1, _Level2, _Level3;  // 이동시킬 3개의 층들
    private int _currLevel;         // 현재 층 수

    private void Start()
    {
        _currLevel = 9;             // 시작할 때 9층에서 시작
    }

    private void OnTriggerEnter(Collider other)     // 계단 중앙에 들어왔을 때 작동
    {
        if(other.CompareTag("Player"))
        {
            if(state == PlayerState.MoveUp)
                MoveUp();
            else
                MoveDown();
        }
    }

    private void MoveUp()
    {
        if(_currLevel < 18)          // 18층은 옥상 구현
        {
            _currLevel++;
            // 층 이동에 따른 오브젝트/몬스터/NPC 비활성화 및 위치 조정 코드 작성
        }
    }

    private void MoveDown()
    {
        if(_currLevel > 1)           // 최하층
        {
            _currLevel--;
            // 층 이동에 따른 오브젝트/몬스터/NPC 비활성화 및 위치 조정 코드 작성
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + new Vector3(0f, 0.25f, 0f), size);
    }
}
