using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundSO", menuName = "Scriptable Objects/PlayerSoundSO")]
public class PlayerSoundSO : ScriptableObject
{
    public AudioClip attackSound;
    public AudioClip attackedSound;
}
