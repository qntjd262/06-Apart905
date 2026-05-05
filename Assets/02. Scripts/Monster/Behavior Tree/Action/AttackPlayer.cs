using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : ActionNode
{
    // 공격 관련
    private float _attackDuration = 1.75f;
    private float _attackTimer = 0f;
    private Vector3 _halfExtents;
    private Vector3 _offset;

    // 레이어 마스크와 공격한 대상을 저장할 HashSet
    private LayerMask _playerMask = LayerMask.GetMask("Player");
    private HashSet<Collider> _colliderHashSet = new HashSet<Collider>();

    public AttackPlayer(Vector3 halfExtents, Vector3 offset, Blackboard blackboard)
    {
        _halfExtents = halfExtents;
        _offset = offset;
        _blackboard = blackboard;
    }

    public override void OnStart()
    {
        _blackboard.MonsterState = Blackboard.State.Attack;
        _blackboard.Self.transform.LookAt(_blackboard.Player.transform);
        _colliderHashSet.Clear();

        // TODO : 애니메이션 실행
        _blackboard.Animator.OnAttack();
        _attackTimer = Time.time;
    }

    public override NodeState OnUpdate()
    {
        var rotOffset = _blackboard.Self.transform.rotation * _offset;
        // 공격 판정을 위한 센터값과 생성할 박스 사이즈
        var center = _blackboard.Center.position + _blackboard.Self.transform.forward + rotOffset; ;

        float attackTime = Time.time - _attackTimer;

        // 공격 지속시간 이내라면
        if (attackTime <= _attackDuration)
        {
            // 공격 시간이 0.2 ~ 0.7초 이내라면
            if (attackTime >= 0.2f && attackTime <= 0.7f)
            {
                Collider[] colliders = Physics.OverlapBox(center, _halfExtents, _blackboard.Self.transform.rotation, _playerMask);

                foreach (var collider in colliders)
                {
                    // 이번 공격 때 이미 충돌한 물체가 아니라면 && 플레이어라면
                    if (!_colliderHashSet.Contains(collider) && collider.CompareTag("Player"))
                    {
                        _colliderHashSet.Add(collider);

                        // TODO : 플레이어 정보 받아와 플레이어에게 공격하는 함수 호출
                        collider.GetComponent<PlayerStat>()?.TakeDamage(_blackboard.MonsterStat.monsterStatSO.damage);
                        Debug.Log("플레이어 공격");
                    }
                }
            }
            return NodeState.Running;
        }

        // 공격 후 잠시 Wait

        // 공격이 끝나면 초기화
        OnStop();
        return NodeState.Success;
    }

    public override void OnStop()
    {
        base.OnStop();
        _attackTimer = 0f;
        _colliderHashSet.Clear();
        _blackboard.MonsterState = Blackboard.State.Idle;
    }
}
