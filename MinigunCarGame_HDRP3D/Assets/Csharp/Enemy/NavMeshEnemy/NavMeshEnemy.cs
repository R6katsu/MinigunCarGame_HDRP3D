using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NavMeshを使う敵
/// </summary>
public class NavMeshEnemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent = null;

    [SerializeField]
    private Transform _target = null;

    // タワーを最終目標とするものの、その他にも地形破壊やタレット破壊を可能としたい

    private void Update()
    {
        if (!_agent.isOnNavMesh) { return; }

        if (_agent.isOnOffMeshLink)
        {
            NavMeshLink link = (NavMeshLink)_agent.navMeshOwner;

            Vector3 endPosWorld
            = link.gameObject.transform.TransformPoint(link.endPoint);

            Vector3 h = transform.localScale.y * Vector3.up;

            _agent.transform.position = endPosWorld + h;
        }

        _agent.destination = _target.position;

        // 目的地を設定
        _agent.SetDestination(_target.position);

        // 経路探索が成功したか確認
        if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("通れる道があります。");
            _agent.isStopped = false;
        }
        else
        {
            Debug.Log("通れる道がありません。");
            _agent.isStopped = true;
        }
    }
}
