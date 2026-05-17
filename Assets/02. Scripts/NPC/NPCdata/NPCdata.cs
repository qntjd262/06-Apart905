using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC/NPCData")]
public class NPCdata : ScriptableObject
{
    [Header("기본 정보")]
    public Sprite NpcImage;
    public string NpcName;
}
