using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.AI.Navigation;

/// <summary>
/// NavMesh���g���G
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshEnemy : MonoBehaviour, IHealth
{
    [SerializeField]
    private NavMeshAgent _agent = null;

    [SerializeField]
    private UnityEngine.UI.Slider _hpBar = null;

    public Transform target = null;

    [SerializeField, Header("���S���Prefab")]
    private Transform _afterDeathPrefab = null;

    [SerializeField]
    private HealthInfo _healthInfo = new();

    [SerializeField]
    private AudioClip[] _hitSEs = new AudioClip[0];

    private float _gravity = -9.81f;  // �d�͂̑傫���i�n����̏d�͂��V�~�����[�g�j
    private float _fallSpeed = 0f;    // ���݂̗������x

    HealthInfo IHealth.GetHealth()
    {
        return _healthInfo;     // ���S���ɃI�u�W�F�N�g�ɂȂ�
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _healthInfo.ChangeHPAction = () =>
        {
            _hpBar.value = _healthInfo.HPValue;

            SoundDirector.Instance.SeVolume = 0.5f;
            SoundDirector.Instance.PlaySe(_hitSEs[UnityEngine.Random.Range(0, _hitSEs.Length)]);
        };

        _healthInfo.DeathAction = () =>
        {
            Transform afterDeath = Instantiate(_afterDeathPrefab);
            afterDeath.transform.position = transform.position;
            afterDeath.transform.rotation = transform.rotation;

            NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
            surface.UpdateNavMesh(surface.navMeshData);

            Destroy(gameObject);
        };
    }

    private void Update()
    {
        if (!_agent.isOnNavMesh)
        {
            // NavMesh�̏�ɂ��Ȃ�
            // �������x���v�Z
            _fallSpeed += _gravity * Time.deltaTime;

            // �I�u�W�F�N�g���������Ɉړ�
            transform.position += new Vector3(0, _fallSpeed * Time.deltaTime, 0);

            return; 
        }

        _agent.nextPosition = transform.position;

        // NavMesh�̏�ɂ���
        _fallSpeed = 0;

        /*  Link�̏�ɋ��鎞�̏���
        if (_agent.isOnOffMeshLink)
        {
            NavMeshLink link = (NavMeshLink)_agent.navMeshOwner;

            Vector3 endPosWorld = link.gameObject.transform.TransformPoint(link.endPoint);

            Vector3 h = transform.localScale.y * Vector3.up;

            _agent.transform.position = endPosWorld + h;
        }
        */

        if (target == null) { return; }

        // �ړI�n��ݒ�
        _agent.SetDestination(target.position);

        // �o�H�T���������������m�F
        if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            _agent.isStopped = false;
        }
        else
        {
            _agent.isStopped = true;
        }
    }
}
