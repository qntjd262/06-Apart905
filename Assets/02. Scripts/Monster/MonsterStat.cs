using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    public MonsterStatSO monsterStatSO;
    private Blackboard _blackBoard;

    [Header("몬스터 체력")]
    private float monsterHealth;
    [SerializeField] private float currentHealth;

    private void Awake()
    {
        monsterHealth = monsterStatSO.maxHP;
    }

    private void Start()
    {
        currentHealth = monsterHealth;
        _blackBoard = GetComponent<TestMonsterAI>().blackboard;
    }


    public void TakeDamage(float damage, GameObject player)
    {
        if (currentHealth < 0)
            return;

        currentHealth -= damage;
       
        Debug.Log($"{damage} 가격 , 남은 체력 {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }

        _blackBoard.Player = player;
        _blackBoard.IsAttacked = true;
        
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
