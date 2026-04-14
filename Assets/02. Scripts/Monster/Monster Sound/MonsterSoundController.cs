using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MonsterSoundController : MonoBehaviour
{
    // 각 좀비마다 별개의 사운드를 가지는 Scriptable Object
    [SerializeField] private MonsterSoundSO monsterSoundSO;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnAttackedSound()
    {
        _audioSource.PlayOneShot(monsterSoundSO.attackedSound);
    }

    public void OnAttackSound()
    {
        _audioSource.PlayOneShot(monsterSoundSO.attackSound);
    }

    public void OnGrowlSound()
    {
        _audioSource.PlayOneShot(monsterSoundSO.growlSound);
    }

    public void OnRageSound()
    {
        _audioSource.PlayOneShot(monsterSoundSO.attackSound);
    }
}
