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

    // 애니메이터 파라미터
    public static readonly int MonsterAniParamIsMoving = Animator.StringToHash("IsMoving");
    public static readonly int MonsterAniParamAttack = Animator.StringToHash("Attack");
    public static readonly int MonsterAniParamAttacked = Animator.StringToHash("Attacked");
    public static readonly int MonsterAniParamDeath = Animator.StringToHash("Death");
    public static readonly int MonsterAniParamIsFront = Animator.StringToHash("IsFront");

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
        _animator.SetBool(MonsterAniParamIsMoving, true);
        _soundController.OnGrowlSound();
    }

    public void OnAttack()
    {
        _animator.SetTrigger(MonsterAniParamAttack);
        _animator.SetBool(MonsterAniParamIsMoving, false);
        _soundController.OnAttackSound();
    }

    public void OnIdle()
    {
        _animator.SetBool(MonsterAniParamIsMoving, false);
    }

    public void OnAttacked(bool isFront)
    {
        _animator.SetBool(MonsterAniParamIsFront, isFront);
        _animator.SetTrigger(MonsterAniParamAttacked);
        _animator.ResetTrigger(MonsterAniParamAttack);
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
}
