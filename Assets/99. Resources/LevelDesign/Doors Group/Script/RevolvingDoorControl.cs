using System.Collections;
using UnityEngine;

public class RevolvingDoorControl : MonoBehaviour, IInteractable
{
    [Header("문 개폐 설정")]
    public float openAngle = 90;
    public float openDuration = 1;

    [Header("문 개폐 사운드")]
    public AudioClip open;
    public AudioClip close;

    private AudioSource audioSource;

    private Quaternion openRotation;
    private Quaternion closeRotation;

    public bool isOpen = false;
    private bool isOpening = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        closeRotation = transform.localRotation;
        openRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + openAngle,
            transform.localEulerAngles.z
        );
    }

    public void Interact(PlayerStat player)
    {
        if (isOpening) return;

        isOpen = !isOpen;
        Quaternion targetRotation = isOpen ? openRotation : closeRotation;
        
        if (isOpen) OnOpenSound();
        else OnCloseSound();


        StartCoroutine(RotateDoor(targetRotation));
    }

    IEnumerator RotateDoor(Quaternion targetRotation)
    {
        isOpening = true;

        float elapsedTime = 0f;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / openDuration);
            yield return null;
        }

        transform.localRotation = targetRotation;
        isOpening = false;
    }

    private void OnOpenSound()
    {
        audioSource.PlayOneShot(open);
    }

    private void OnCloseSound()
    {
        audioSource.PlayOneShot(close);
    }
}
