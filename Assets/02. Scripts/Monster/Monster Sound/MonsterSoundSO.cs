using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSound", menuName = "Scriptable Objects/MonsterSoundData")]
public class MonsterSoundSO : ScriptableObject
{
    public AudioClip attackSound;
    public AudioClip attackedSound;
    public AudioClip growlSound;
    public AudioClip rageSound;
}
