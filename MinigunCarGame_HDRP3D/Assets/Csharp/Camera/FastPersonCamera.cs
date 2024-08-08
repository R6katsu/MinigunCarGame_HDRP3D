using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviour
{
    [Tooltip("追いかける対象"), SerializeField]
    Transform follow = null;

    [Tooltip("マウス感度"), SerializeField, Min(1.0f)]
    float mouseSensitivity = 1.0f;

    //x軸、y軸中心の回転値
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private void OnEnable()
    {
        // カーソル非表示
        Cursor.visible = false;

        // カーソルのロック
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        //追従対象と同じpositionに瞬間移動
        transform.position = follow.position;

        //マウスの横移動の距離を足し続ける
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        //マウスの縦移動の距離を引き続ける
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -60f, 60f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0);
    }
}
