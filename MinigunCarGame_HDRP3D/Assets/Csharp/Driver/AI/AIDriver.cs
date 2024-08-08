using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 自動運転によって動く
/// </summary>
public class AIDriver : DriverBehaviour
{
    /// <summary>
    /// 前か後か
    /// </summary>
    public enum FrontOrBehind
    {
        None,
        [Tooltip("前")] Front,
        [Tooltip("後")] Behind
    }

    /// <summary>
    /// 右か左か
    /// </summary>
    public enum RightOrLeft
    {
        None,
        [Tooltip("右")] Right,
        [Tooltip("左")] Left,
        [Tooltip("真っすぐ")] Straight
    }

    // SerializeField
    [SerializeField, Min(0.0f), Header("速度上限")]
    private float _speedLimit = 0.0f;

    [SerializeField, Header("目標位置を子に持つ親Transform")]
    private Transform _objectiveTransformsParent = null;

    // private
    private bool _isSteering = false;
    private bool _isAccelerator = false;
    private bool _isBrake = false;

    private int _currentDestinationNumber = 0;

    [Tooltip("目標位置のリスト")]
    private List<Transform> _objectiveTransforms = new();

    override protected void OnEnable()
    {
        base.OnEnable();

        // 目標位置を子に持つ親Transformを探索し、目標位置のリストを作成する
        foreach (Transform transform in _objectiveTransformsParent)
        {
            _objectiveTransforms.Add(transform);
        }

        StartCoroutine(Accelerator(1.0f));
    }

    private FrontOrBehind _currentFrontOrBehind = 0;
    private RightOrLeft _currentRightOrLeft = 0;

    private RightOrLeft _lastTimeRightOrLeft = 0;

    private FrontOrBehind CurrentFrontOrBehind
    {
        get => _currentFrontOrBehind;
        set
        {
            if (_currentFrontOrBehind != value)
            {
                _currentFrontOrBehind = value;
                ChangeFrontOrBehind();
            }
        }
    }

    private RightOrLeft CurrentRightOrLeft
    {
        get => _currentRightOrLeft;
        set
        {
            if (_currentRightOrLeft != value)
            {
                _currentRightOrLeft = value;
                ChangeRightOrLeft();
            }
        }
    }

    private void ChangeFrontOrBehind()
    {
        float accelerator = 0.0f;
        accelerator = 1.0f;
        switch (CurrentFrontOrBehind)
        {
            case FrontOrBehind.Front:
                //accelerator = 1.0f;
                break;

            case FrontOrBehind.Behind:
                //accelerator = 0.0f;

                if (CurrentRightOrLeft == RightOrLeft.Straight)
                {
                    int random = Mathf.RoundToInt(UnityEngine.Random.value);

                    if (random == 1)
                    {
                        CurrentRightOrLeft = RightOrLeft.Right;
                    }
                    else
                    {
                        CurrentRightOrLeft = RightOrLeft.Left;
                    }
                }
                break;

            default:
                break;
        }

        //StartCoroutine(Accelerator(accelerator));
    }

    private void ChangeRightOrLeft()
    {
        float steering = 0.0f;
        switch (CurrentRightOrLeft)
        {
            case RightOrLeft.Right:
                steering = 1.0f;
                break;

            case RightOrLeft.Left:
                steering = -1.0f;
                break;

            case RightOrLeft.Straight:
                if (CurrentFrontOrBehind != FrontOrBehind.Front)
                {
                    if (!_isSteering)
                    {
                        StartCoroutine(Steering(steering));
                    }

                    return;
                }

                HorizontalCanceled();

                // 前回は目標地点に対して真っすぐではなかった
                if (_lastTimeRightOrLeft != RightOrLeft.Straight)
                {
                    // 回転を 0にする
                    Vector3 angle = Rigidbody.angularVelocity;
                    angle.y = 0.0f;
                    Rigidbody.angularVelocity = angle;

                    // ステアリングを適用する車輪配列
                    var steerWheels = DriveTypeWheels(_wheelsParameter, _wheelsParameter.WheelsSteeringType);

                    // 全ての車輪のステアリングを 0にする
                    for (int i = 0; i < steerWheels.Count; i++)
                    {
                        steerWheels[i].WheelCollider.steerAngle = 0.0f;
                    }
                }
                break;

            default:
                break;
        }

        _lastTimeRightOrLeft = CurrentRightOrLeft;      // 今回のRightOrLeftを次の前回のRightOrLeftに代入

        HorizontalCanceled();
        StartCoroutine(Steering(steering));
    }

    /// <summary>
    /// 目標地点は前後どちらの方向にあるか、左右どちらの方向にあるかを判定し結果を返す
    /// </summary>
    /// <param name="destination">目標地点</param>
    /// <returns>目標地点は前後どちらの方向にあるか、左右どちらの方向にあるか</returns>
    private (FrontOrBehind, RightOrLeft) GetTargetDirection(Transform destination)
    {
        // 目標地点は前後どちらの方向にあるか、左右どちらの方向にあるか
        FrontOrBehind responseFrontOrBehind = FrontOrBehind.Front;
        RightOrLeft responseRightOrLeft = RightOrLeft.Right;

        // 左右を判定するための閾値を定義
        float sideDotThreshold = 0.1f;

        // 現在の位置から目的地までの方向ベクトルを計算
        Vector3 directionToDestination = destination.position - transform.position;

        // ベクトルの長さを無視して方向だけを取得するために正規化
        directionToDestination.Normalize();

        // 正面を向いているかどうかをドット積で判定
        float dotProduct = Vector3.Dot(transform.forward, directionToDestination);

        // 左右の方向を判定
        float crossProductY = Vector3.Cross(transform.forward, directionToDestination).y;

        // 目標地点が前後どちらの方向にあるか
        if (dotProduct > 0)
        {
            // Dotが正の値の時は前方に目標がある
            responseFrontOrBehind = FrontOrBehind.Front;
        }
        else
        {
            // Dotが負の値の時は後方に目標がある
            responseFrontOrBehind = FrontOrBehind.Behind;
        }

        // 目標地点が左右どちらの方向にあるか
        if (crossProductY > sideDotThreshold)
        {
            // 目標が右にある
            responseRightOrLeft = RightOrLeft.Right;
        }
        else if (crossProductY < -sideDotThreshold)
        {
            // 目標が左にある
            responseRightOrLeft = RightOrLeft.Left;
        }
        else
        {
            // 目標に対して真っすぐ
            responseRightOrLeft = RightOrLeft.Straight;
        }

        return (responseFrontOrBehind, responseRightOrLeft);
    }

    private void Update()
    {
        (FrontOrBehind frontOrBehind, RightOrLeft rightOrLeft) targetDirection = GetTargetDirection(_objectiveTransforms[_currentDestinationNumber]);

        CurrentFrontOrBehind = targetDirection.frontOrBehind;
        CurrentRightOrLeft = targetDirection.rightOrLeft;

        // 次の目標との距離
        float arrivalThreshold = 10.0f; // 目的地到達とみなす距離の閾値

        Vector3 targetPoint = _objectiveTransforms[_currentDestinationNumber].position;
        targetPoint.y = transform.position.y;

        // 現在の位置と目的地の位置との距離を計算
        float distanceToDestination = Vector3.Distance(transform.position, targetPoint);

        // 距離が到達閾値以下かどうかを判定
        if (distanceToDestination <= arrivalThreshold)
        {
            int x = UnityEngine.Random.Range(-30, 30);
            int z = UnityEngine.Random.Range(-30, 30);
            _objectiveTransforms[0].position = new Vector3(x, 0, z);
            /*
            _currentDestinationNumber++;

            // 目的地の数を超過
            if (_objectiveTransforms.Count <= _currentDestinationNumber)
            {
                _currentDestinationNumber = 0;
            }
            */
        }
    }

    private void HorizontalCanceled()
    {
        _isSteering = false;

        // ステアリング駆動を0.0fにする
        FinishSteering(_wheelsParameter);
    }

    private void VerticalCanceled()
    {
        _isAccelerator = false;

        // アクセル駆動を0.0fにする
        FinishAccelerator(_wheelsParameter);
    }

    private void BrakeStarted()
    {
        StartCoroutine(Brake());
    }

    private void BrakeCanceled()
    {
        _isBrake = false;

        // ブレーキを0.0fにする
        FinishBrake(_wheelsParameter);
    }

    /// <summary>
    /// ステアリング
    /// </summary>
    /// <returns></returns>
    private IEnumerator Steering(float value)
    {
        // 操舵中なら切り上げる
        if (_isSteering) { yield break; }

        _isSteering = true;

        // FixedUpdate
        while (_isSteering)
        {
            ApplySteering(_wheelsParameter, value);

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// アクセル
    /// </summary>
    /// <returns></returns>
    private IEnumerator Accelerator(float value)
    {
        // 移動中なら切り上げる
        if (_isAccelerator) { yield break; }

        _isAccelerator = true;

        // FixedUpdate
        while (_isAccelerator)
        {
            float speedLimit = _speedLimit;

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = _objectiveTransforms[_currentDestinationNumber].position;

            // 現在の向きを取得
            Vector3 currentForward = transform.forward;

            // 目標への方向ベクトルを計算
            Vector3 directionToTarget = (targetPosition - currentPosition).normalized;

            // 現在の向きと目標への方向ベクトルの間の角度を計算
            float entryAngle = Vector3.Angle(currentForward, directionToTarget);

            // 現在の向きから見て左か右かを判定
            float sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(currentForward, directionToTarget)));
            float signedAngle = entryAngle * sign;

            // 誤差10を超過していたら速度制限を厳しくする
            if (Mathf.Abs(signedAngle) > 15.0f)
            {
                speedLimit = 7.5f;
            }
            else if (Mathf.Abs(signedAngle) > 10.0f)
            {
                speedLimit = 10.0f;
            }

            if (_objectiveTransforms.Count > _currentDestinationNumber + 1)
            {
                // 次の目標の角度
                Vector3 vectorA = transform.position;
                Vector3 vectorB = _objectiveTransforms[_currentDestinationNumber].position;
                Vector3 vectorC = _objectiveTransforms[_currentDestinationNumber + 1].position;

                // vectorBを基点としたvectorAとvectorCの方向ベクトルを計算
                Vector3 AB = vectorA - vectorB;
                Vector3 CB = vectorC - vectorB;

                // 角度を求める
                float angle = Vector3.Angle(AB, CB);

                vectorB.y = vectorA.y;

                // 現在の位置と目的地の位置との距離を計算
                float distanceToDestination = Vector3.Distance(vectorA, vectorB);

                // 速度が超過しており、且つ近ければ速度上限を下げる
                if (angle <= 100.0f && distanceToDestination <= 20.0f)
                {
                    speedLimit = 7.5f;
                }
            }

            ApplyAcceleratorWithSpeedLimit(_wheelsParameter, value, speedLimit);

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 上限速度付きアクセル駆動
    /// </summary>
    /// <param name="wheelsParameter">車輪のパラメーター</param>
    /// <param name="direction">方向</param>
    /// <param name="speedLimit">上限速度</param>
    private void ApplyAcceleratorWithSpeedLimit(WheelsParameter wheelsParameter, float direction, float speedLimit)
    {
        base.ApplyAccelerator(wheelsParameter, direction);

        // アクセルを適用する車輪配列
        var acceleWheels = DriveTypeWheels(wheelsParameter, wheelsParameter.WheelsAcceleratorType);

        // アクセルを反映
        foreach (var acceleWheel in acceleWheels)
        {
            if (speedLimit < Speed)
            {
                // 前進速度が上限速度を超過している場合は減速
                acceleWheel.WheelCollider.motorTorque = 0;
                continue;
            }
        }
    }

    /// <summary>
    /// ブレーキ
    /// </summary>
    private IEnumerator Brake()
    {
        // 減速中なら切り上げる
        if (_isBrake) { yield break; }

        _isBrake = true;

        // FixedUpdate
        while (_isBrake)
        {
            ApplyBrake(_wheelsParameter);

            yield return new WaitForFixedUpdate();
        }
    }
}
