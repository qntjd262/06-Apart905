using System;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    /* 구현된 3개 층의 문에 할당하는 스크립트
     */

    [SerializeField] private Vector3 openRotation;     // 문이 열렸을 때의 회전값
    [SerializeField] private Vector3 closedRotation;   // 문이 닫혔을 때의 회전값
    [SerializeField] private float animDuration = 0.5f; // 애니메이션 재생 시간(초)
    [SerializeField] private AnimationCurve animCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // 이징 커브

    public int localFloorOffset; // 3개 층 중 몇 번째 층인지 (0,1,2)
    public bool isLeft;
    public int doorNum;

    private bool isOpen = false;
    private bool isAnimating = false;   // 애니메이션 진행 중 중복 입력 방지
    private Coroutine _animCoroutine;
    private DoorManager _doorManager;

    public string GetInteractText()
    {
        return isOpen ? "문 닫기" : "문 열기";
    }

    public Constants.InteractType GetInteractType()
    {
        return Constants.InteractType.Door;
    }

    public void Interact(PlayerStat player)
    {
        if (isAnimating) return;

        isOpen = !isOpen;

        DoorInfo info = GetInfo();
        _doorManager.SetDoorOpen(info, isOpen);

        PlayDoorAnimation(isOpen);
    }

    // 층 이동 시 애니메이션 없이 즉시 상태 설정
    public void SetStateImmediate(bool open)
    {
        // 진행 중인 애니메이션이 있으면 중단
        if (_animCoroutine != null)
        {
            StopCoroutine(_animCoroutine);
            _animCoroutine = null;
            isAnimating = false;
        }

        isOpen = open;
        transform.rotation = Quaternion.Euler(open ? openRotation : closedRotation);
    }

    private void PlayDoorAnimation(bool open)
    {
        if (_animCoroutine != null)
            StopCoroutine(_animCoroutine);

        _animCoroutine = StartCoroutine(AnimateDoor(open));
    }

    private IEnumerator AnimateDoor(bool open)
    {
        isAnimating = true;

        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.Euler(open ? openRotation : closedRotation);

        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float curvedT = animCurve.Evaluate(t);

            transform.rotation = Quaternion.Lerp(from, to, curvedT);
            yield return null;
        }

        transform.rotation = to;

        isAnimating = false;
        _animCoroutine = null;
    }

    private DoorInfo GetInfo() => new DoorInfo
    {
        isLeft = isLeft,
        doorNum = doorNum
    };
}