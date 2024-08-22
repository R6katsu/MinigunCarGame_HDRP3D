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

    [Tooltip("マウス感度"), SerializeField, Min(1.0f)]
    float mouseSensitivity = 1.0f;

    //x軸、y軸中心の回転値
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
        // カーソル非表示
        Cursor.visible = false;

        // カーソルのロック
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
        // マウスホイールの回転量を取得
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
            // マウスホイールを前方に回転させた
            currentBlockType = (BlockNameType)(((int)currentBlockType) + 1);

            // 超過したら最初に戻る
            if ((int)currentBlockType > Enum.GetValues(typeof(BlockNameType)).Length - 1)
            {
                currentBlockType = (BlockNameType)1;
            }

            currentBlockTypeUGUI.text = currentBlockType.ToString();
        }
        else if (scroll < 0f)
        {
            // マウスホイールを後方に回転させた
            currentBlockType = (BlockNameType)(((int)currentBlockType) - 1);

            // 設置対象から除外している要素まで小さくなっていた場合は一番最後の要素に変更
            if (currentBlockType == (BlockNameType)0)
            {
                currentBlockType = (BlockNameType)Enum.GetValues(typeof(BlockNameType)).Cast<BlockNameType>().Last();
            }

            currentBlockTypeUGUI.text = currentBlockType.ToString();
        }

        // 一人称Camera
        //マウスの横移動の距離を足し続ける
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        //マウスの縦移動の距離を引き続ける
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
            // 破壊
            BlockAction.BreakBlock(hit, transform.position);
        }
    }

    private void MouseClickRight()
    {
        Ray playerRay = new Ray(selfCamera.transform.position, selfCamera.transform.forward);
        RaycastHit hit;

        // もしもそれが構造物記録ブロックだったら設置ではなく使用
        if (Physics.Raycast(playerRay, out hit, interactionRayLength, groundMask))
        {
            // 設置
            PlacementTerrain(hit, transform.position);

            //StructureRecord(this BlockAction blockAction)
        }
    }

    /// <summary>
    /// 設置
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="sourcePosition">Rayの発射位置</param>
    private void PlacementTerrain(RaycastHit hit, Vector3 sourcePosition)
    {
        float rayLengthening = 0.1f;    // Rayをブロック方向に延長

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
