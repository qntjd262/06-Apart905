using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class MonsterAnimator : MonoBehaviour
{   
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Rigidbody _rigidbody;

    private MonsterSoundController _soundController;
    private Coroutine _moanCoroutine;

    // 애니메이터 파라미터
    public static readonly int MonsterAniParamIsMoving = Animator.StringToHash("IsMoving");
    public static readonly int MonsterAniParamAttack = Animator.StringToHash("Attack");
    public static readonly int MonsterAniParamAttacked = Animator.StringToHash("Attacked");
    public static readonly int MonsterAniParamDeath = Animator.StringToHash("Death");
    public static readonly int MonsterAniParamIsFront = Animator.StringToHash("IsFront");
    public static readonly int MonsterAniParamChase = Animator.StringToHash("Chase");
    public static readonly int MonsterAniParamWalkState = Animator.StringToHash("WalkState");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _soundController = GetComponent<MonsterSoundController>();
        _rigidbody = GetComponent<Rigidbody>();

        _animator.applyRootMotion = true;
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = _animator.rootPosition;
        rootPosition.y = _navMeshAgent.nextPosition.y;
        transform.position = rootPosition;
        _navMeshAgent.nextPosition = rootPosition;
    }

    public void OnWalk()
    {
        _animator.SetInteger(MonsterAniParamWalkState, 1);
        _soundController.OnGrowlSound();
    }

    public void OnAttack()
    {
        _animator.SetTrigger(MonsterAniParamAttack);
        _animator.SetInteger(MonsterAniParamWalkState, 0);
        _soundController.OnAttackSound();
    }

    public void OnIdle()
    {
        _animator.SetInteger(MonsterAniParamWalkState, 0);
        if (_moanCoroutine == null)
        {
            _moanCoroutine = StartCoroutine(MoanRoutine());
        }
    }

    public void OnChase()
    {
        _animator.SetInteger(MonsterAniParamWalkState, 2);
        _soundController.OnRageSound();
    }

    IEnumerator MoanRoutine()
    {
        _soundController.OnMoanSound();
        Debug.Log("moan 코루틴 시작");
        while (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
        {
            float randomDelay = Random.Range(5f, 10f);
            yield return new WaitForSeconds(randomDelay);

            float randomExecute = Random.Range(0, 2);
            if (_animator.GetBool(MonsterAniParamIsMoving) == false &&
                randomExecute == 1)
            {
                Debug.Log("moan 출력");
                _soundController.OnMoanSound();
            }
        }

        _moanCoroutine = null;
        yield break;
    }

    public void OnAttacked(bool isFront)
    {
        _animator.SetBool(MonsterAniParamIsFront, isFront);
        _animator.SetTrigger(MonsterAniParamAttacked);
        _animator.ResetTrigger(MonsterAniParamAttack);
        _animator.SetInteger(MonsterAniParamWalkState, 0);
        _soundController.OnAttackedSound();
        _rigidbody.isKinematic = false;

        StartCoroutine(SetKinematicTrue());
    }

    IEnumerator SetKinematicTrue()
    {
        while(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        _rigidbody.isKinematic = true;
    }

    public void OnDeath()
    {
        _animator.SetTrigger(MonsterAniParamDeath);
    }

    public void OnRage()
    {
        _soundController.OnRageSound();
    }
}
