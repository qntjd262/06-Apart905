using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private PlayerSoundSO soundSO;

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

    public void OnAttackSound() => PlaySound(soundSO.attackSound);
    public void OnAttackedSound() => PlaySound(soundSO.attackedSound);
}
