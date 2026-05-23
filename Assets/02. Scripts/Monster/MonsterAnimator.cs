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

    private void FixedUpdate()
    {
        if (_navMeshAgent.enabled)
        {
            SynchronizeAnimatorAndAgent();
        }
    }

    private void SynchronizeAnimatorAndAgent()
    {
        // navmesh가 가려하는 위치와 현재 위치간의 차이
        Vector3 worldDeltaPosition = _navMeshAgent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        // 몬스터 기준으로 목표 방향을 구한다
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // 부드럽게 이동하게
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.02f);
      
        // navmesh 경로와 애니메이션의 위치가 많이 벗어날 시, 보정
        if (worldDeltaPosition.magnitude > _navMeshAgent.radius / 3f)
        {
            transform.position = Vector3.Lerp(transform.position, _navMeshAgent.nextPosition, smooth);
            //transform.position = Vector3.MoveTowards(transform.position, _navMeshAgent.nextPosition, Time.deltaTime * 5f);
        }
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
        while (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
        {
            float randomDelay = Random.Range(5f, 10f);
            yield return new WaitForSeconds(randomDelay);

            float randomExecute = Random.Range(0, 2);
            if (_animator.GetBool(MonsterAniParamIsMoving) == false &&
                randomExecute == 1)
            {
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
