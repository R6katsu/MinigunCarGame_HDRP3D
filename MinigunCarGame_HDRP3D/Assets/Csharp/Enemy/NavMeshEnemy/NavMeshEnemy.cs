using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.AI.Navigation;

/// <summary>
/// NavMeshを使う敵
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshEnemy : MonoBehaviour, IHealth
{
    [SerializeField]
    private NavMeshAgent _agent = null;

    [SerializeField]
    private UnityEngine.UI.Slider _hpBar = null;

    public Transform target = null;

    [SerializeField, Header("死亡後のPrefab")]
    private Transform _afterDeathPrefab = null;

    [SerializeField]
    private HealthInfo _healthInfo = new();

    [SerializeField]
    private AudioClip[] _hitSEs = new AudioClip[0];

    private float _gravity = -9.81f;  // 重力の大きさ（地球上の重力をシミュレート）
    private float _fallSpeed = 0f;    // 現在の落下速度

    HealthInfo IHealth.GetHealth()
    {
        return _healthInfo;     // 死亡時にオブジェクトになる
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
            // NavMeshの上にいない
            // 落下速度を計算
            _fallSpeed += _gravity * Time.deltaTime;

            // オブジェクトを下方向に移動
            transform.position += new Vector3(0, _fallSpeed * Time.deltaTime, 0);

            return; 
        }

        _agent.nextPosition = transform.position;

        // NavMeshの上にいる
        _fallSpeed = 0;

        /*  Linkの上に居る時の処理
        if (_agent.isOnOffMeshLink)
        {
            NavMeshLink link = (NavMeshLink)_agent.navMeshOwner;

            Vector3 endPosWorld = link.gameObject.transform.TransformPoint(link.endPoint);

            Vector3 h = transform.localScale.y * Vector3.up;

            _agent.transform.position = endPosWorld + h;
        }
        */

        if (target == null) { return; }

        // 目的地を設定
        _agent.SetDestination(target.position);

        // 経路探索が成功したか確認
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
