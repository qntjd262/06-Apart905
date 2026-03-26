using UnityEngine;

public enum ItemType
{
    Equipable, //장비 아이템
    Eatable,   //음식 및 사용 아이템
    Useable    //핵심 아이템(퀘스트)
}

public enum EatableType
{
    Hunger,   //허기
    Thirst,   //갈증
    Health,   //체력
    Stamina,  //스태미너
    Infection //감염도
}

[System.Serializable]
public class ItemDataEatable
{
    public EatableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;         //아이템 이름
    public string description;      //아이템 설명
    public ItemType type;           //아이템 타입
    public Sprite icon;             //아이콘
    public GameObject dropPrefab;   //프리팹 정보

    [Header("Stacking")]
    public bool canStack;           //중첩으로 가질수 잇는지
    public int maxStackAmount;      //가질수 잇다면, 최대 중첩은 몇개인지

    [Header("Eatable")]
    public ItemDataEatable[] eatables;  //허기,갈증,스태미나,감염도 구분

    [Header("Equip")]
    public GameObject equipPrefab;
}
