using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
    /// <summary>
    /// �ԗւ̈ʒu
    /// </summary>
    public enum WheelPosition
    {
        [Tooltip("�O��")] FrontWheel,
        [Tooltip("���")] RearWheel
    }

    // �ڒn���Ă��邩

    [SerializeField, Header("�ԗւ̈ʒu")]
    private WheelPosition _wheelPosition = 0;

    [SerializeField, Header("�ԗւ�z�u����ʒu")]
    private Transform _wheelOffset = null;

    /// <summary>
    /// �ԗւ̈ʒu
    /// </summary>
    public WheelPosition ItsWheelPosition => _wheelPosition;

    /// <summary>
    /// WheelCollider
    /// </summary>
    public WheelCollider WheelCollider { get; private set; } = null;

    /// <summary>
    /// �ԗւ�z�u����ʒu
    /// </summary>
    public Transform WheelOffset => _wheelOffset;

    private void OnEnable()
    {
        WheelCollider = GetComponent<WheelCollider>();  // RequireComponent

        // �^�C���̔��a��sphereCol�Ŏ擾�A�s�v�ɂȂ���sphereCol���폜
        SphereCollider sphereCol = WheelOffset.gameObject.AddComponent<SphereCollider>();

        // �ԗւ̍ő�̑傫�����擾�AsphereCol�̔��a�ɂ����Ĕ��a���擾
        float wheelSscale = Mathf.Max(WheelOffset.localScale.x, WheelOffset.localScale.y, WheelOffset.localScale.z);
        WheelCollider.radius = sphereCol.radius * wheelSscale;
        Destroy(sphereCol);

        // wheelCol���ԗփ��f���Ɠ��ʒu���p�x�ɕύX
        transform.localPosition = WheelOffset.localPosition;
        transform.localEulerAngles = WheelOffset.localEulerAngles;
    }
}
