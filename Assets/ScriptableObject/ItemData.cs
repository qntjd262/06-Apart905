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
    None, Hunger, Thirst, Health, Stamina, Infection
}



public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string ID;
    public string Name;
    public string itemName;
    public int SpawnCount;
    public ItemType itemType;
    public Sprite icon;
    [TextArea] public string description;

}

public class EatableItemData : ItemData
{
    public EatableType eatableType_1;
    public float value_1;
    public EatableType eatableType_2;
    public float value_2;
}

public class EquipItemData : ItemData
{
    public float equipValue;
    
    public GameObject equipPrefab;
}

public class UseItemData : ItemData
{
    //특 수 아이템 공통 데이터 value
    public float useValue;
}
