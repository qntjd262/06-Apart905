using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private float attackRange = 2f;
    //추후 playerstat.cs 추가 후 수정 예정 임시 공격력
    [SerializeField] private float attackPower = 4f;

    [SerializeField] private Transform cameraPos;

    void Awake()
    {
        //playerstat.cs에서 캐릭터 스탯 가져오기
    }

    public void Attack()
    {
        if(cameraPos == null) return;

        Ray ray = new Ray(cameraPos.position, cameraPos.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, attackRange))
        {
            Debug.Log($"{hit.collider.name},거리 : {hit.distance}");
            //TODO : 몬스터 데미지 적용 + 애니메이션에서 Attack()함수 호출하기
            if (hit.collider.CompareTag("Monster"))
            {
                MonsterTest monsterTest = hit.collider.GetComponent<MonsterTest>();

                if(monsterTest != null)
                {
                    monsterTest.TakeDamage(attackPower);
                }
            }
        }
        else if(hit.collider != null)
        {
            Debug.Log($"없음, 현재 레이캐스트 : {hit.collider.name}");
        }
        else
        {
            Debug.Log("없음");
        }
    }

    private void OnDrawGizmos()
    {
        if(cameraPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(cameraPos.position, cameraPos.position +(cameraPos.forward * attackRange));
        }
    }
}
