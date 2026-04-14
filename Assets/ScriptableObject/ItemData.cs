using UnityEngine;

// 아이템의 대분류
public enum ItemType
{
    Equipable, // 장착 아이템 (무기, 손전등 등)
    Eatable,   // 소모 아이템 (음식, 약 등)
    Useable    // 특수 아이템 (열쇠, 퀘스트 아이템 등)
}

// 소모 시 영향을 줄 스탯 종류
public enum EatableType
{
    Hunger, Thirst, Health, Stamina, Infection
}

[System.Serializable]
public class ItemDataEatable
{
    public EatableType type;
    public float value;
}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/ItemData")]

public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    public string itemCode;
    [TextArea] public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab; // 바닥에 떨어져 있을 때 모델링

    [Header("소모품 설정 (Eatable일 때만 사용)")]
    public ItemDataEatable[] eatables;

    [Header("장착 설정 (Equipable일 때만 사용)")]
    public GameObject equipPrefab; // 손에 들었을 때 모델링

    [Header("스택 설정")]
    public int maxStack = 1;
}
