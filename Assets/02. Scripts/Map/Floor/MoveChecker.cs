using UnityEngine;

public class MoveChecker : MonoBehaviour
{
    private MoveLevel moveLevel;

    public bool isDown, isMid, isUp;

    private void Awake()
    {
        moveLevel = FindFirstObjectByType<MoveLevel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (!moveLevel.check)
            {
                if (isDown)
                    moveLevel.state = MoveLevel.PlayerState.MoveDown;
                else if (isMid)
                    moveLevel.check = true;
                else if (isUp)
                        moveLevel.state = MoveLevel.PlayerState.MoveUp;
            }
            else
            {
                if (isDown)
                {
                    if (moveLevel.state == MoveLevel.PlayerState.MoveDown)
                    {
                        moveLevel.check = false;
                        return;
                    }
                    else if (moveLevel.state == MoveLevel.PlayerState.MoveUp)
                    {
                        moveLevel.check = false;
                        moveLevel.MoveUp();
                        moveLevel.state = MoveLevel.PlayerState.MoveDown;
                    }
                }
                else if (isUp)
                {
                    if (moveLevel.state == MoveLevel.PlayerState.MoveUp)
                    {
                        moveLevel.check = false;
                        return;
                    }
                    else if (moveLevel.state == MoveLevel.PlayerState.MoveDown)
                    {
                        moveLevel.check = false;
                        moveLevel.MoveDown();
                        moveLevel.state = MoveLevel.PlayerState.MoveUp;
                    }
                }
            }
        }

        // 몬스터일 때, 몬스터가 위층으로 가는지 아래로 가는지 판별
        // 몬스터와 체크trigger의 y축 차이로 판별한다
        if (other.CompareTag("Monster"))
        {
            var targetMonster = other.GetComponent<MonsterController>();
            if (isMid)
            {
                float dir = targetMonster.transform.position.y - transform.position.y;
                moveLevel.MonsterFloorMove(targetMonster, dir < 0);
            }
        }
    }
}
