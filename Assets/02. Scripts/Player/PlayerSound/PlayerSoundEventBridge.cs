using UnityEngine;

public class PlayerSoundEventBridge : MonoBehaviour
{
    [SerializeField] private PlayerSoundController soundController;

    private void Awake()
    {
        soundController = GetComponentInParent<PlayerSoundController>();
    }

    public void OnAttack() => soundController.OnAttackSound();
}
