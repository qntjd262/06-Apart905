using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    //TODO : 무기 개발 완료 후 공격 범위 가져오기
    [SerializeField] private float attackRange = 2f;
    //TODO : playerstat.cs 추가 후 수정 예정 임시 공격력
    private PlayerStat playerStat;

    [SerializeField] private Transform cameraPos;

    [Header("공격 쿨 + 애니메이션 적용 타임")]
    //TODO : 애니메이션 적용 할 때 타임 맞추기
    private float attackDelay = 0.25f;
    private float attackCoolDown = 1.45f;
    public bool isAttacking {get; private set;}

    [Header("공격 소음")]
    private PlayerNoise playerNoise;
    private float attackNoiseRadius = 10f;

    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }


    void Awake()
    {
        //TODO : playerstat.cs에서 캐릭터 스탯 가져오기
        playerStat = GetComponent<PlayerStat>();

        playerNoise = GetComponent<PlayerNoise>();
    }

    public void Attack()
    {

        if(isAttacking) return;

        StartCoroutine(AttackRoutine());

    }

    //공격 코루틴
    IEnumerator AttackRoutine()
    {
        if(anim != null) anim.SetTrigger("IsAttack");
        isAttacking = true;

        //attackDelay = 애니메이션 동작 타임 제어
        yield return new WaitForSeconds(attackDelay);
        if(playerNoise != null) playerNoise.TriggerOneShotNoise(attackNoiseRadius);

        AttackRayCast();

        //attackCoolDown = 공격 쿨타임
        yield return new WaitForSeconds(attackCoolDown);

        isAttacking = false;
    }

    //공격 판정 + 데미지 적용
    private void AttackRayCast()
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
                MonsterController monster = hit.collider.GetComponent<MonsterController>();

                if(monster != null && playerStat != null)
                {
                    monster.TakeDamage(playerStat.AttackPower, this.gameObject);
                    Debug.Log("공격 성공");
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

    //사거리를 보기 위함
    private void OnDrawGizmos()
    {
        if(cameraPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(cameraPos.position, cameraPos.position +(cameraPos.forward * attackRange));
        }
    }
}
