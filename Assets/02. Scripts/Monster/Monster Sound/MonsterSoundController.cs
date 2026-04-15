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
        _audioSource.clip = monsterSoundSO.attackedSound;
        _audioSource.Play();
    }

    public void OnAttackSound()
    {
        _audioSource.clip = monsterSoundSO.attackSound;
        _audioSource.Play();
    }

    public void OnGrowlSound()
    {
        _audioSource.clip = monsterSoundSO.growlSound;
        _audioSource.Play();
    }

    public void OnRageSound()
    {
        _audioSource.clip = monsterSoundSO.rageSound;
        _audioSource.Play();
    }
}
