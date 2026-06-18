using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MonsterSoundController : MonoBehaviour
{
    // 각 좀비마다 별개의 사운드를 가지는 Scriptable Object
    [SerializeField] private MonsterSoundSO soundSO;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    // 다른 소리 실행 시 끊김
    public void OnAttackedSound() => PlaySound(soundSO.attackedSound);
    public void OnAttackSound() => PlaySound(soundSO.attackedSound);

    // 중간에 끊기지 않고 소리가 발생
    public void OnGrowlSound() => _audioSource.PlayOneShot(soundSO.growlSound);
    public void OnRageSound() => _audioSource.PlayOneShot(soundSO.rageSound);
    public void OnMoanSound() => _audioSource.PlayOneShot(soundSO.moanSound);
}
