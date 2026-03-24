using System.Collections.Generic;
using UnityEngine;

public class TestMonsterAI : MonoBehaviour
{
    private Node        _rootNode;
    private GameObject  _detectedPlayer;
    private Vector3     _originPos;

    [Header("AI 설정 값")]
    [SerializeField] private float      _detectRadius = 5f;
    [SerializeField] private LayerMask  _enemyLayer;
    [SerializeField] private float      _moveSpeed = 2f;

    private void Awake()
    {
        _originPos = transform.position; // 초기 위치 저장
    }

    private void Start()
    {
        // Behavior Tree 생성
        var rootNode = new SelectorNode
            (
                new SequenceNode
                (
                    new ActionNode(CheckDetectedEnemy),
                    new ActionNode(ChaseDetectedEnemy)
                ),
                    
                new ActionNode(ReturnOriginPosition)
            );

        _rootNode = rootNode;
    }

    private void Update()
    {
        _rootNode.Evaluate(); // 매 프레임마다 Behavior Tree 평가
    }

    // 범위 내에 적을 탐색
    private NodeState CheckDetectedEnemy()
    {
        // 본 몬스터를 중심으로 한 구체를 생성
        var overlapSphere = Physics.OverlapSphere(transform.position, _detectRadius, _enemyLayer); 

        // 구체 안에 플레이어가 탐지되면 _detectedPlayer에 저장 및 Success 반환 
        if (overlapSphere != null && overlapSphere.Length > 0) 
        {
            _detectedPlayer = overlapSphere[0].gameObject;
            return NodeState.Success;
        }

        // 적이 없다면 failure 반환
        return NodeState.Failure; 
    }

    // 탐지된 적을 추적
    private NodeState ChaseDetectedEnemy()
    {
        // 탐지된 적이 없다면 Failure 반환
        if (_detectedPlayer == null) return NodeState.Failure; 

        // 플레이어와 몬스터의 거리가 0.5이하이면 Success 반환
        if (Vector3.Magnitude(transform.position - _detectedPlayer.transform.position) < 0.5f) 
        {
            return NodeState.Success;
        }

        // 플레이어 쪽으로 이동
        transform.position = Vector3.MoveTowards(transform.position, _detectedPlayer.transform.position, _moveSpeed * Time.deltaTime);

        // 거리가 아직 멀다면 Running 반환
        return NodeState.Running; 
    }

    // 원래 위치로 복귀
    private NodeState ReturnOriginPosition()
    {
        Debug.Log("제자리 복귀 시작");

        // 원래 위치와의 거리가 0.5 이하면 Success 반환
        if (Vector3.Magnitude(transform.position - _originPos) < 0.5f) 
        {
            return NodeState.Success;
        }

        transform.position = Vector3.MoveTowards(transform.position, _originPos, _moveSpeed * Time.deltaTime);

        // 아직 원래 위치로 돌아가지 못했다면 Running 반환
        return NodeState.Running; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectRadius); // 탐지 범위를 시각적으로 표시
    }
}
