using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : ActionNode
{
    // 애니메이터
    private Animator _animator;

    // 공격 관련
    private float _attackDuration = 0.8f;
    private float _attackTimer = 0f;
    private Vector3 _halfExtents;

    // 레이어 마스크와 공격한 대상을 저장할 HashSet
    private LayerMask _playerMask = LayerMask.GetMask("Player");
    private HashSet<Collider> _colliderHashSet = new HashSet<Collider>();

    public AttackPlayer(Vector3 halfExtents, Blackboard blackboard)
    {
        _blackboard = blackboard;
        _halfExtents = halfExtents;
        _animator = _blackboard.Animator;
    }

    public override NodeState Evaluate()
    {
        // 공격 판정을 위한 센터값과 생성할 박스 사이즈
        var center = _blackboard.Center.position + _blackboard.Self.transform.forward;

        // 공격 시작 시 초기화
        if (!_blackboard.IsAttacking)
        {
            _blackboard.IsAttacking = true;
            _attackTimer = Time.time;
            _colliderHashSet.Clear();
            Debug.Log("공격 시작");
            // TODO : 애니메이션 실행
        }

        // 공격 지속시간 이내라면
        if (Time.time - _attackTimer <= _attackDuration)
        {
            _blackboard.IsAttacking = true;
            // 몬스터 앞에 상자를 생성해 충돌한 물체 배열에 담음
            Collider[] colliders = Physics.OverlapBox(center, _halfExtents, _blackboard.Self.transform.rotation, _playerMask);

            foreach (var collider in colliders)
            {
                // 공격 시간이 0.2 ~ 0.7초 이내라면
                // 이번 공격 때 이미 충돌한 물체가 아니라면 && 플레이어라면
                if (Time.time - _attackTimer >= 0.2f && Time.time - _attackTimer <= 0.7f &&
                    !_colliderHashSet.Contains(collider) && collider.CompareTag("Player"))
                {
                    _colliderHashSet.Add(collider);

                    // TODO : 플레이어 정보 받아와 플레이어에게 공격하는 함수 호출
                    //collider.GetComponent<PlayerStat>().TakeDamage(_blackboard.MonsterStat.damage);

                    Debug.Log("플레이어에게 데미지");
                }
            }

            return NodeState.Running;
        }

        // 공격이 끝나면 초기화
        _attackTimer = 0f;
        _colliderHashSet.Clear();
        _blackboard.IsAttacking = false;
        return NodeState.Success;
    }
}
