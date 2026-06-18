using System;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    /* 구현된 3개 층의 문에 할당하는 스크립트
     */

    [SerializeField] private GameObject Hinge;                  // 문의 회전축이 되는 오브젝트
    [SerializeField] private DoorManager _doorManager;
    [SerializeField] private Vector3 openRotation;              // 문이 열렸을 때의 회전값
    [SerializeField] private Vector3 closedRotation;            // 문이 닫혔을 때의 회전값
    [SerializeField] private float animDuration = 0.5f;         // 애니메이션 재생 시간(초)

    public int realFloor;
    public bool isLeft;
    public int doorNum;

    private bool isOpen = false;
    private bool isAnimating = false;   // 애니메이션 진행 중 중복 입력 방지
    private Coroutine _animCoroutine;    
    private AnimationCurve animCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // 이징 커브

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
        Hinge.transform.localRotation = Quaternion.Euler(open ? openRotation : closedRotation);
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

        Quaternion from = Hinge.transform.localRotation;
        Quaternion to = Quaternion.Euler(open ? openRotation : closedRotation);

        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float curvedT = animCurve.Evaluate(t);

            Hinge.transform.localRotation = Quaternion.Lerp(from, to, curvedT);
            yield return null;
        }

        Hinge.transform.localRotation = to;
        isAnimating = false;
        _animCoroutine = null;
        Debug.Log($"[Door] 애니메이션 완료: {(open ? "열림" : "닫힘")}");
    }

    private DoorInfo GetInfo() => new DoorInfo
    {
        floor = realFloor,
        isLeft = isLeft,
        doorNum = doorNum
    };
}