using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData itemData; 

    public void Interact(PlayerStat player) // 매개변수 타입 변경
    {
        if (itemData == null) return;

        if (itemData.type == ItemType.Eatable)
        {
            foreach (var effect in itemData.eatables)
            {
                ApplyEffect(player, effect);
            }
        }
        Destroy(gameObject);
    }

private void ApplyEffect(PlayerStat player, ItemDataEatable effect)
{
    StatCondition targetStat = null;

    switch (effect.type)
    {
        case EatableType.Hunger: 
            targetStat = player.hunger; 
            break;
        case EatableType.Thirst: 
            targetStat = player.thirst; 
            break;
        case EatableType.Health: 
            targetStat = player.hp; 
            break;
        case EatableType.Stamina: 
            targetStat = player.stamina; 
            break;
        case EatableType.Infection: 
            targetStat = player.infection; 
            break;
    }

    if (targetStat != null)
    {
        // 감염도(Infection)인 경우에만 수치를 뺌
        if (effect.type == EatableType.Infection)
        {
            targetStat.currentValue -= effect.value;
        }
        else // 나머지는 수치를 더합니다 (회복 효과)
        {
            targetStat.currentValue += effect.value;
        }

        // 스탯이 0 ~ 최대값 범위를 벗어나지 않게 고정
        targetStat.currentValue = Mathf.Clamp(targetStat.currentValue, 0, targetStat.maxValue);
    }
}
}
