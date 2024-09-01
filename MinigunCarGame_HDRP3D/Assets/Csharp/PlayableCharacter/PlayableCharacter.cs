using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayableInput))]
public class PlayableCharacter : MonoBehaviour
{
    /// <summary>
    /// 移動の状態
    /// </summary>
    private enum MoveState
    {
        None,
        [Tooltip("何もしていない")] IsIdling,
        [Tooltip("地上移動")] IsGroundMove,
        [Tooltip("空中移動")] IsAirMove,
        [Tooltip("ジャンプ中")] IsJumpMove
    }

    #region Field
    private Collider _selfCollider = null;
    private Transform _selfTransform = null;
    private PlayableInput _playableInput = null;

    [Tooltip("移動入力の方向")]
    private Vector3 _movementInputVector = Vector3.zero;

    [SerializeField, Header("重力")]
    private float _gravity = 9.81f;

    [SerializeField, Header("ジャンプ速度")]
    private float _jumpSpeed = 0.0f;
    [SerializeField]
    private LayerMask _groundLayer = 0;
    private MoveState _moveState = MoveState.None;

    private float targetAngleY = 0.0f;
    private float angleY = 0.0f;
    private float _turnVelocity = 0.0f;

    [SerializeField, Header("移動速度")]
    private float _moveSpeed = 0.0f;
    #endregion

    #region Get/Set
    /// <summary>
    /// 移動入力の方向
    /// </summary>
    private Vector3 MovementInputVector
    {
        get => _movementInputVector;
        set
        {
            // 変更があった
            if (value != _movementInputVector)
            {
                _movementInputVector = value;
            }
        }
    }

    /// <summary>
    /// 現在の移動状態
    /// </summary>
    private MoveState CurrentMoveState
    {
        get => _moveState;
        set
        {
            if (_moveState != value)
            {
                _moveState = value;

                ChangeCurrentMoveState();
            }
        }
    }
    #endregion

    private void OnEnable()
    {
        // RequireComponent
        _selfCollider = GetComponent<Collider>();
        _selfTransform = GetComponent<Transform>();
        _playableInput = GetComponent<PlayableInput>();

        // 入力による処理を定義
        _playableInput.HorizontalStartedAction = HorizontalStarted;
        _playableInput.HorizontalCanceledAction = HorizontalCanceled;
        _playableInput.VerticalStartedAction = VerticalStarted;
        _playableInput.VerticalCanceledAction = VerticalCanceled;
        _playableInput.JumpStartedAction = JumpStarted;

        CurrentMoveState = MoveState.IsIdling;
    }

    private void Update()
    {
        // 以下は回転処理

        // 移動入力がある場合は、振り向き動作も行う
        if (_movementInputVector != Vector3.zero)
        {
            // y軸周りの目標角度[deg]を計算
            targetAngleY = -Mathf.Atan2(_movementInputVector.z, _movementInputVector.x)
            * Mathf.Rad2Deg + 90;

            // イージングしながら次の回転角度[deg]を計算
            angleY = Mathf.SmoothDampAngle(
                _selfTransform.eulerAngles.y,
                targetAngleY,
                ref _turnVelocity,
                0.25f
            );
        }

        // オブジェクトの回転を更新
        _selfTransform.rotation = Quaternion.Euler(0, angleY, 0);
    }

    #region Method
    private void ChangeCurrentMoveState()
    {
        switch (CurrentMoveState)
        { 
            case MoveState.None:
                break;
            case MoveState.IsIdling:
                // 地上&無入力の時は最低限の処理に留める
                StartCoroutine(IsIdling());
                break;
            case MoveState.IsGroundMove:
                // 地上&移動
                StartCoroutine(IsGroundMove());
                break;
            case MoveState.IsAirMove:
                // ジャンプをnullに設定
                _playableInput.JumpStartedAction = null;

                // 空中&重力と移動
                StartCoroutine(IsAirMove());
                break;
            case MoveState.IsJumpMove:
                // ジャンプをnullに設定
                _playableInput.JumpStartedAction = null;

                // ジャンプを実行
                StartCoroutine(IsJumpMove(0.5f));
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError("想定されたMoveStateではなかった");
#endif
                break;
        }
    }

    /// <summary>
    /// 空中に居る場合はIsAirMoveに変更。操作があった場合はIsGroundMoveに変更。
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator IsIdling()
    {
        // IsGroundIdling中は実行
        while (CurrentMoveState == MoveState.IsIdling)
        {
            yield return null;

            // 空中にいる
            if (!Physics.CheckSphere(transform.position + Vector3.down, 0.5f, _groundLayer))
            {
                CurrentMoveState = MoveState.IsAirMove;
                break;
            }

            // 地上&操作があった
            if (_movementInputVector != Vector3.zero)
            {
                CurrentMoveState = MoveState.IsGroundMove;

                // ジャンプを有効化
                _playableInput.JumpStartedAction = JumpStarted;
                break;
            }
        }
    }

    /// <summary>
    /// 重力を考慮しない前後左右への移動を実行する。空中だった場合はIsAirMoveに変更。
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator IsGroundMove()
    {
        // IsGroundMove中は実行
        while (CurrentMoveState == MoveState.IsGroundMove)
        {
            yield return null;

            // 空中だった
            if (!Physics.CheckSphere(transform.position + Vector3.down, 0.5f, _groundLayer))
            {
                CurrentMoveState = MoveState.IsAirMove;
                break;
            }

            var movementInputVector = _movementInputVector;

            // 操作入力ベクトルを正規化
            if (movementInputVector.magnitude > 1.0f)
            {
                movementInputVector.Normalize();
            }

            // y軸に対する入力を 0に設定
            movementInputVector.y = 0.0f;

            // 正規化したベクトルに移動速度を適用
            Vector3 velocity = movementInputVector * _moveSpeed;

            velocity.y = 0.0f;

            // 現在フレームの移動量を移動速度から計算
            transform.position += velocity * Time.deltaTime;
        }
    }

    /// <summary>
    /// 重力を考慮して前後左右への移動を実行する。地上だった場合はIsGroundIdlingに変更。
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator IsAirMove()
    {
        float fallSpeed = 0.0f;    // 現在の落下速度

        // IsAirMove中は実行
        while (CurrentMoveState == MoveState.IsAirMove)
        {
            yield return null;

            // 地上だった
            if (Physics.CheckSphere(transform.position + Vector3.down, 0.5f, _groundLayer))
            {
                CurrentMoveState = MoveState.IsIdling;
                break;
            }

            var movementInputVector = _movementInputVector;

            // 操作入力ベクトルを正規化
            if (movementInputVector.magnitude > 1.0f)
            {
                movementInputVector.Normalize();
            }

            // y軸に対する入力を 0に設定
            movementInputVector.y = 0.0f;

            // 正規化したベクトルに移動速度を適用
            Vector3 velocity = movementInputVector * _moveSpeed;

            // 落下速度を計算
            fallSpeed -= _gravity * Time.deltaTime;

            // 重力を適用
            velocity.y = fallSpeed;

            // 現在フレームの移動量を移動速度から計算
            transform.position += velocity * Time.deltaTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns>null</returns>
    private IEnumerator IsJumpMove(float time)
    {
        float elapsedTime = 0.0f;
        float riseSpeed = _jumpSpeed;     // 現在の上昇速度(徐々に減速)

        // ここでActionを定義する
        // Action内では接地判定の有無を判定し、その有無によってStateを変更する

        while (CurrentMoveState == MoveState.IsJumpMove)
        {
            yield return null;

            var movementInputVector = _movementInputVector;

            // 操作入力ベクトルを正規化
            if (movementInputVector.magnitude > 1.0f)
            {
                movementInputVector.Normalize();
            }

            // y軸に対する入力を 0に設定
            movementInputVector.y = 0.0f;

            // 正規化したベクトルに移動速度を適用
            Vector3 velocity = movementInputVector * _moveSpeed;

            // 上昇速度を計算
            riseSpeed -= _jumpSpeed * Time.deltaTime;

            // 上昇を適用
            velocity.y = riseSpeed;

            // 現在フレームの移動量を移動速度から計算
            transform.position += velocity * Time.deltaTime;

            // 実際にはアニメーションカーブで自然にジャンプさせる

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= time)
            {
                // 地上/空中で分岐する処理に移行
                CurrentMoveState = MoveState.IsIdling;
            }
        }

        // 頭をぶつけるか、時間経過で終える
    }

    /// <summary>
    /// 左右の入力があった時に実行される処理
    /// </summary>
    /// <param name="context"></param>
    private void HorizontalStarted(InputAction.CallbackContext context)
    {
        var movementInputVector = MovementInputVector;
        movementInputVector.x = context.ReadValue<float>();
        MovementInputVector = movementInputVector;
    }

    private void HorizontalCanceled(InputAction.CallbackContext context)
    {
        var movementInputVector = MovementInputVector;
        movementInputVector.x = 0.0f;
        MovementInputVector = movementInputVector;
    }

    /// <summary>
    /// 上下の入力があった時に実行される処理
    /// </summary>
    /// <param name="context"></param>
    private void VerticalStarted(InputAction.CallbackContext context)
    {
        var movementInputVector = MovementInputVector;
        movementInputVector.z = context.ReadValue<float>();
        MovementInputVector = movementInputVector;
    }

    private void VerticalCanceled(InputAction.CallbackContext context)
    {
        var movementInputVector = MovementInputVector;
        movementInputVector.z = 0.0f;
        MovementInputVector = movementInputVector;
    }

    private void JumpStarted(InputAction.CallbackContext context)
    {
        CurrentMoveState = MoveState.IsJumpMove;
    }
    #endregion
}
