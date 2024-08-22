using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NavMesh���g���G
/// </summary>
public class NavMeshEnemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent = null;

    [SerializeField]
    private Transform _target = null;

    // �^���[���ŏI�ڕW�Ƃ�����̂́A���̑��ɂ��n�`�j���^���b�g�j����\�Ƃ�����

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

        // �ړI�n��ݒ�
        _agent.SetDestination(_target.position);

        // �o�H�T���������������m�F
        if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("�ʂ�铹������܂��B");
            _agent.isStopped = false;
        }
        else
        {
            Debug.Log("�ʂ�铹������܂���B");
            _agent.isStopped = true;
        }
    }
}
