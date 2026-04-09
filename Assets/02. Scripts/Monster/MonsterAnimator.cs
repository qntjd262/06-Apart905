using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAnimator : MonoBehaviour
{
    public Animator _animator;
    private NavMeshAgent _navMeshAgent;

    // 애니메이터 파라미터
    public static readonly int MonsterAniParamIsMoving = Animator.StringToHash("IsMoving");
    public static readonly int MonsterAniParamAttack = Animator.StringToHash("Attack");
    public static readonly int MonsterAniParamAttacked = Animator.StringToHash("Attacked");
    public static readonly int MonsterAniParamDeath = Animator.StringToHash("Death");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

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
        _animator.SetBool(MonsterAniParamIsMoving, true);
    }

    public void OnAttack()
    {
        _animator.SetTrigger(MonsterAniParamAttack);
        _animator.SetBool(MonsterAniParamIsMoving, false);
    }

    public void OnIdle()
    {
        _animator.SetBool(MonsterAniParamIsMoving, false);
    }

    public void OnAttacked()
    {
        _animator.SetTrigger(MonsterAniParamAttacked);
        _animator.ResetTrigger(MonsterAniParamAttack);
    }

    public void OnDeath()
    {
        _animator.SetTrigger(MonsterAniParamDeath);

    }
}
