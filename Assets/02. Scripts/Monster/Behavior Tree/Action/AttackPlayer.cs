using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : ActionNode
{
    // 애니메이터
    private Animator _animator;

    // 공격 주기 관련
    private float _attackDuration = 0.8f;
    private float _attackTimer = 0f;

    // 레이어 마스크와 공격한 대상을 저장할 HashSet
    private LayerMask _playerMask = LayerMask.GetMask("Player");
    private HashSet<Collider> _colliderHashSet = new HashSet<Collider>();

    public AttackPlayer(Blackboard blackboard)
    {
        _blackboard = blackboard;
        _animator = _blackboard.Animator;
    }

    public override NodeState Evaluate()
    {
        // 공격 판정을 위한 센터값과 생성할 박스 사이즈
        var center = _blackboard.Self.transform.position + (_blackboard.Self.transform.forward * 1f);
        var halfExtents = new Vector3(0.5f, 1f, 0.5f);

        Debug.Log("공격 중");
        // 공격 지속시간 이내라면
        if (_attackTimer <= _attackDuration)
        {
            _blackboard.IsAttacking = true;
            // 몬스터 앞에 상자를 생성해 충돌한 물체 배열에 담음
            Collider[] colliders = Physics.OverlapBox(center, halfExtents, _blackboard.Self.transform.rotation, _playerMask);

            foreach (var collider in colliders)
            {
                // 이번 공격 때 이미 충돌한 물체가 아니라면 && 플레이어라면
                // 공격 시간이 0.2 ~ 0.7초 이내라면
                if (!_colliderHashSet.Contains(collider) && collider.CompareTag("Player")
                    && _attackTimer >= 0.2f && _attackTimer <= 0.7f)
                {
                    _colliderHashSet.Add(collider);
                    Debug.Log("플레이어에게 데미지");
                }
            }
            _attackTimer += Time.deltaTime;

            return NodeState.Running;
        }
        else
        {
            _attackTimer = 0f;
            _colliderHashSet.Clear();
            _blackboard.IsAttacking = false;
            Debug.Log("공격 끝");
            return NodeState.Success;
        }
     
    }
}
