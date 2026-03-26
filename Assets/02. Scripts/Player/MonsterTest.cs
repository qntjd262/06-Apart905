using UnityEngine;

public class MonsterTest : MonoBehaviour
{
    [Header("몬스터 체력")]
    [SerializeField] private float monsterHealth = 20f;
    [SerializeField] private float currentHealth;

    void Start()
    {
        currentHealth = monsterHealth;
    }

    public void TakeDamage(float damage)
    {
        if(currentHealth < 0)
            return;
        
        currentHealth -= damage;

        Debug.Log($"{damage} 가격 , 남은 체력 {currentHealth}");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
