using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform minigun;

    Vector3 currentPos;//現在のカメラ位置
    Vector3 pastPos;//過去のカメラ位置

    private Vector3 diff;//移動距離

    private void OnEnable()
    {
        // カーソル非表示
        Cursor.visible = false;

        // カーソルのロック
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        //最初のプレイヤーの位置の取得
        pastPos = player.transform.position;
    }
    private void Update()
    {
        //プレイヤーの現在地の取得
        currentPos = player.position;

        diff = currentPos - pastPos;

        transform.position = Vector3.Lerp(transform.position, transform.position + diff, 1.0f);//カメラをプレイヤーの移動差分だけうごかすよ

        pastPos = currentPos;

        // マウスの移動量を取得
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        // X方向に一定量移動していれば横回転
        if (Mathf.Abs(mx) > 0.01f)
        {
            // 回転軸はワールド座標のY軸
            transform.RotateAround(player.position, Vector3.up, mx);
        }

        // Y方向に一定量移動していれば縦回転
        if (Mathf.Abs(my) > 0.01f)
        {
            // 現在のカメラのY軸回転角度を取得
            float currentYAngle = transform.eulerAngles.x;

            // -180〜180度の範囲に補正
            if (currentYAngle > 180f)
                currentYAngle -= 360f;

            // Y軸回転を制限
            currentYAngle -= my;
            currentYAngle = Mathf.Clamp(currentYAngle, -10, 10);

            // 回転軸はカメラ自身のX軸
            transform.localEulerAngles = new Vector3(currentYAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        // プレイヤーのY軸をカメラの向きに合わせる
        RotatePlayerToCamera();

        RotateMinigunToCamera();
    }

    private void RotatePlayerToCamera()
    {
        Vector3 cameraForward = transform.forward;
        cameraForward.y = 0; // 水平方向のみ
        cameraForward.Normalize();

        if (cameraForward.sqrMagnitude > 0.01f)
        {
            player.rotation = Quaternion.LookRotation(cameraForward);
        }
    }

    private void RotateMinigunToCamera()
    {
        Vector3 cameraForward = transform.forward;
        cameraForward.Normalize();

        if (cameraForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            // 現在の回転角度を取得
            Vector3 currentEulerAngles = minigun.rotation.eulerAngles;

            // 現在の角度が180より大きい場合は-180〜180度の範囲に補正
            currentEulerAngles.x = NormalizeAngle(currentEulerAngles.x);
            targetEulerAngles.x = NormalizeAngle(targetEulerAngles.x);

            // 新しい回転角度を計算し、X軸の回転角度を制限
            float newXAngle = Mathf.Clamp(targetEulerAngles.x, -15.0f, 15.0f);

            // 制限された角度を適用
            Quaternion limitedRotation = Quaternion.Euler(newXAngle, targetEulerAngles.y, targetEulerAngles.z);
            minigun.localRotation = limitedRotation;
        }
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
