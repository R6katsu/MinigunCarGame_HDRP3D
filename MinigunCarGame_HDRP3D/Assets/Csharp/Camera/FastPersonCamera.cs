using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviour
{
    [Tooltip("�ǂ�������Ώ�"), SerializeField]
    Transform follow = null;

    [Tooltip("�}�E�X���x"), SerializeField, Min(1.0f)]
    float mouseSensitivity = 1.0f;

    //x���Ay�����S�̉�]�l
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private void OnEnable()
    {
        // �J�[�\����\��
        Cursor.visible = false;

        // �J�[�\���̃��b�N
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        //�Ǐ]�ΏۂƓ���position�ɏu�Ԉړ�
        transform.position = follow.position;

        //�}�E�X�̉��ړ��̋����𑫂�������
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        //�}�E�X�̏c�ړ��̋���������������
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -60f, 60f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0);
    }
}
