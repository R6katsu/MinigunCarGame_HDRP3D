using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayableInput))]
public class PlayableCharacter : MonoBehaviour
{
    /// <summary>
    /// �ړ��̏��
    /// </summary>
    private enum MoveState
    {
        None,
        [Tooltip("�������Ă��Ȃ�")] IsIdling,
        [Tooltip("�n��ړ�")] IsGroundMove,
        [Tooltip("�󒆈ړ�")] IsAirMove,
        [Tooltip("�W�����v��")] IsJumpMove
    }

    #region Field
    private Collider _selfCollider = null;
    private Transform _selfTransform = null;
    private PlayableInput _playableInput = null;

    [Tooltip("�ړ����͂̕���")]
    private Vector3 _movementInputVector = Vector3.zero;

    [SerializeField, Header("�d��")]
    private float _gravity = 9.81f;

    [SerializeField, Header("�W�����v���x")]
    private float _jumpSpeed = 0.0f;
    [SerializeField]
    private LayerMask _groundLayer = 0;
    private MoveState _moveState = MoveState.None;

    private float targetAngleY = 0.0f;
    private float angleY = 0.0f;
    private float _turnVelocity = 0.0f;

    [SerializeField, Header("�ړ����x")]
    private float _moveSpeed = 0.0f;
    #endregion

    #region Get/Set
    /// <summary>
    /// �ړ����͂̕���
    /// </summary>
    private Vector3 MovementInputVector
    {
        get => _movementInputVector;
        set
        {
            // �ύX��������
            if (value != _movementInputVector)
            {
                _movementInputVector = value;
            }
        }
    }

    /// <summary>
    /// ���݂̈ړ����
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

        // ���͂ɂ�鏈�����`
        _playableInput.HorizontalStartedAction = HorizontalStarted;
        _playableInput.HorizontalCanceledAction = HorizontalCanceled;
        _playableInput.VerticalStartedAction = VerticalStarted;
        _playableInput.VerticalCanceledAction = VerticalCanceled;
        _playableInput.JumpStartedAction = JumpStarted;

        CurrentMoveState = MoveState.IsIdling;
    }

    private void Update()
    {
        // �ȉ��͉�]����

        // �ړ����͂�����ꍇ�́A�U�����������s��
        if (_movementInputVector != Vector3.zero)
        {
            // y������̖ڕW�p�x[deg]���v�Z
            targetAngleY = -Mathf.Atan2(_movementInputVector.z, _movementInputVector.x)
            * Mathf.Rad2Deg + 90;

            // �C�[�W���O���Ȃ��玟�̉�]�p�x[deg]���v�Z
            angleY = Mathf.SmoothDampAngle(
                _selfTransform.eulerAngles.y,
                targetAngleY,
                ref _turnVelocity,
                0.25f
            );
        }

        // �I�u�W�F�N�g�̉�]���X�V
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
                // �n��&�����͂̎��͍Œ���̏����ɗ��߂�
                StartCoroutine(IsIdling());
                break;
            case MoveState.IsGroundMove:
                // �n��&�ړ�
                StartCoroutine(IsGroundMove());
                break;
            case MoveState.IsAirMove:
                // �W�����v��null�ɐݒ�
                _playableInput.JumpStartedAction = null;

                // ��&�d�͂ƈړ�
                StartCoroutine(IsAirMove());
                break;
            case MoveState.IsJumpMove:
                // �W�����v��null�ɐݒ�
                _playableInput.JumpStartedAction = null;

                // �W�����v�����s
                StartCoroutine(IsJumpMove(0.5f));
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError("�z�肳�ꂽMoveState�ł͂Ȃ�����");
#endif
                break;
        }
    }

    /// <summary>
    /// �󒆂ɋ���ꍇ��IsAirMove�ɕύX�B���삪�������ꍇ��IsGroundMove�ɕύX�B
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator IsIdling()
    {
        // IsGroundIdling���͎��s
        while (CurrentMoveState == MoveState.IsIdling)
        {
            yield return null;

            // �󒆂ɂ���
            if (!Physics.CheckSphere(transform.position + Vector3.down, 0.5f, _groundLayer))
            {
                CurrentMoveState = MoveState.IsAirMove;
                break;
            }

            // �n��&���삪������
            if (_movementInputVector != Vector3.zero)
            {
                CurrentMoveState = MoveState.IsGroundMove;

                // �W�����v��L����
                _playableInput.JumpStartedAction = JumpStarted;
                break;
            }
        }
    }

    /// <summary>
    /// �d�͂��l�����Ȃ��O�㍶�E�ւ̈ړ������s����B�󒆂������ꍇ��IsAirMove�ɕύX�B
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator IsGroundMove()
    {
        // IsGroundMove���͎��s
        while (CurrentMoveState == MoveState.IsGroundMove)
        {
            yield return null;

            // �󒆂�����
            if (!Physics.CheckSphere(transform.position + Vector3.down, 0.5f, _groundLayer))
            {
                CurrentMoveState = MoveState.IsAirMove;
                break;
            }

            var movementInputVector = _movementInputVector;

            // ������̓x�N�g���𐳋K��
            if (movementInputVector.magnitude > 1.0f)
            {
                movementInputVector.Normalize();
            }

            // y���ɑ΂�����͂� 0�ɐݒ�
            movementInputVector.y = 0.0f;

            // ���K�������x�N�g���Ɉړ����x��K�p
            Vector3 velocity = movementInputVector * _moveSpeed;

            velocity.y = 0.0f;

            // ���݃t���[���̈ړ��ʂ��ړ����x����v�Z
            transform.position += velocity * Time.deltaTime;
        }
    }

    /// <summary>
    /// �d�͂��l�����đO�㍶�E�ւ̈ړ������s����B�n�ゾ�����ꍇ��IsGroundIdling�ɕύX�B
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator IsAirMove()
    {
        float fallSpeed = 0.0f;    // ���݂̗������x

        // IsAirMove���͎��s
        while (CurrentMoveState == MoveState.IsAirMove)
        {
            yield return null;

            // �n�ゾ����
            if (Physics.CheckSphere(transform.position + Vector3.down, 0.5f, _groundLayer))
            {
                CurrentMoveState = MoveState.IsIdling;
                break;
            }

            var movementInputVector = _movementInputVector;

            // ������̓x�N�g���𐳋K��
            if (movementInputVector.magnitude > 1.0f)
            {
                movementInputVector.Normalize();
            }

            // y���ɑ΂�����͂� 0�ɐݒ�
            movementInputVector.y = 0.0f;

            // ���K�������x�N�g���Ɉړ����x��K�p
            Vector3 velocity = movementInputVector * _moveSpeed;

            // �������x���v�Z
            fallSpeed -= _gravity * Time.deltaTime;

            // �d�͂�K�p
            velocity.y = fallSpeed;

            // ���݃t���[���̈ړ��ʂ��ړ����x����v�Z
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
        float riseSpeed = _jumpSpeed;     // ���݂̏㏸���x(���X�Ɍ���)

        // ������Action���`����
        // Action���ł͐ڒn����̗L���𔻒肵�A���̗L���ɂ����State��ύX����

        while (CurrentMoveState == MoveState.IsJumpMove)
        {
            yield return null;

            var movementInputVector = _movementInputVector;

            // ������̓x�N�g���𐳋K��
            if (movementInputVector.magnitude > 1.0f)
            {
                movementInputVector.Normalize();
            }

            // y���ɑ΂�����͂� 0�ɐݒ�
            movementInputVector.y = 0.0f;

            // ���K�������x�N�g���Ɉړ����x��K�p
            Vector3 velocity = movementInputVector * _moveSpeed;

            // �㏸���x���v�Z
            riseSpeed -= _jumpSpeed * Time.deltaTime;

            // �㏸��K�p
            velocity.y = riseSpeed;

            // ���݃t���[���̈ړ��ʂ��ړ����x����v�Z
            transform.position += velocity * Time.deltaTime;

            // ���ۂɂ̓A�j���[�V�����J�[�u�Ŏ��R�ɃW�����v������

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= time)
            {
                // �n��/�󒆂ŕ��򂷂鏈���Ɉڍs
                CurrentMoveState = MoveState.IsIdling;
            }
        }

        // �����Ԃ��邩�A���Ԍo�߂ŏI����
    }

    /// <summary>
    /// ���E�̓��͂����������Ɏ��s����鏈��
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
    /// �㉺�̓��͂����������Ɏ��s����鏈��
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
