using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
    [TextArea] public string description;
    public int hp;
    public int atk;
    public int def;
    public int stam;
    public int thirst;
    public int hunger;
    public int sanity;
}