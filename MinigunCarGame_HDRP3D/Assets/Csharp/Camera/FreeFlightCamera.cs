using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static BlockType;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Rigidbody))]
public class FreeFlightCamera : MonoBehaviour
{
    private Camera selfCamera;
    private Rigidbody selfRigidbody;

    public float interactionRayLength = 5;

    public LayerMask groundMask;

    [Tooltip("�}�E�X���x"), SerializeField, Min(1.0f)]
    float mouseSensitivity = 1.0f;

    //x���Ay�����S�̉�]�l
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    public float maxPower = 100;
    public float accel = 2;
    public float toruque;
    [SerializeField]
    private float power = 5;

    private BlockNameType currentBlockType = BlockNameType.GrassDirt;

    [SerializeField]
    private Canvas currentBlockTypeCanvas = null;
    private TextMeshProUGUI currentBlockTypeUGUI = null;

    private void OnEnable()
    {
        // �J�[�\����\��
        Cursor.visible = false;

        // �J�[�\���̃��b�N
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Awake()
    {
        if (selfCamera == null)
            selfCamera = GetComponent<Camera>();

        if (selfRigidbody == null)
            selfRigidbody = GetComponent<Rigidbody>();

        currentBlockTypeUGUI = Instantiate(currentBlockTypeCanvas).GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        // �}�E�X�z�C�[���̉�]�ʂ��擾
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(0))
        {
            MouseClickLeft();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            MouseClickRight();
        }
        else if (scroll > 0f)
        {
            // �}�E�X�z�C�[����O���ɉ�]������
            currentBlockType = (BlockNameType)(((int)currentBlockType) + 1);

            // ���߂�����ŏ��ɖ߂�
            if ((int)currentBlockType > Enum.GetValues(typeof(BlockNameType)).Length - 1)
            {
                currentBlockType = (BlockNameType)1;
            }

            currentBlockTypeUGUI.text = currentBlockType.ToString();
        }
        else if (scroll < 0f)
        {
            // �}�E�X�z�C�[��������ɉ�]������
            currentBlockType = (BlockNameType)(((int)currentBlockType) - 1);

            // �ݒu�Ώۂ��珜�O���Ă���v�f�܂ŏ������Ȃ��Ă����ꍇ�͈�ԍŌ�̗v�f�ɕύX
            if (currentBlockType == (BlockNameType)0)
            {
                currentBlockType = (BlockNameType)Enum.GetValues(typeof(BlockNameType)).Cast<BlockNameType>().Last();
            }

            currentBlockTypeUGUI.text = currentBlockType.ToString();
        }

        // ��l��Camera
        //�}�E�X�̉��ړ��̋����𑫂�������
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        //�}�E�X�̏c�ړ��̋���������������
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -60f, 60f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0);
    }

    private void MouseClickLeft()
    {
        Ray playerRay = new Ray(selfCamera.transform.position, selfCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(playerRay, out hit, interactionRayLength, groundMask))
        {
            // �j��
            BlockAction.BreakBlock(hit, transform.position);
        }
    }

    private void MouseClickRight()
    {
        Ray playerRay = new Ray(selfCamera.transform.position, selfCamera.transform.forward);
        RaycastHit hit;

        // ���������ꂪ�\�����L�^�u���b�N��������ݒu�ł͂Ȃ��g�p
        if (Physics.Raycast(playerRay, out hit, interactionRayLength, groundMask))
        {
            // �ݒu
            PlacementTerrain(hit, transform.position);

            //StructureRecord(this BlockAction blockAction)
        }
    }

    /// <summary>
    /// �ݒu
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="sourcePosition">Ray�̔��ˈʒu</param>
    private void PlacementTerrain(RaycastHit hit, Vector3 sourcePosition)
    {
        float rayLengthening = 0.1f;    // Ray���u���b�N�����ɉ���

        Vector3 pos_obj = (hit.point - hit.normal * rayLengthening) + hit.normal;
        Vector3Int hitWorldCoord = new Vector3Int
        (
            Mathf.RoundToInt(pos_obj.x),
            Mathf.RoundToInt(pos_obj.y),
            Mathf.RoundToInt(pos_obj.z)
        );

        BlockAction.PlacementBlock(hitWorldCoord, currentBlockType);
    }
}
