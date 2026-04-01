using Unity.Mathematics;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterStatSO data;

    [Header("Current Status")]
    public float currentHp;
    public float currentStamina;
    public float currentHunger;
    public float currentThirst;
    public float currentInfection;

    private void Start()
    {
        if(data != null)
        {
            currentHp = data.Hp;
            currentStamina = data.MaxStamina;
            currentHunger = data.MaxHunger;
            currentThirst = data.MaxThirst;
            currentInfection = 0;
        }
    }

    private void Update()
    {
        if(currentHunger > 0)
            currentHunger -= data.HungerDecreaseRate * Time.deltaTime;

        if(currentThirst > 0)
            currentThirst -= data.ThirstDecreaseRate * Time.deltaTime;

        if(currentInfection < data.MaxInfection)
            currentInfection += data.InfectionIncreaseRate * Time.deltaTime;
        
        currentHunger = Mathf.Clamp(currentHunger, 0, data.MaxHunger);
        currentThirst = Mathf.Clamp(currentThirst, 0, data.MaxThirst);
        currentInfection = Mathf.Clamp(currentInfection, 0, data.MaxInfection);
    }
}
