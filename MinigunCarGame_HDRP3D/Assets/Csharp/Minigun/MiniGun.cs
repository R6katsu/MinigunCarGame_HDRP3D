using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MiniGun : MonoBehaviour
{
    // �n�ʂɓ����������͏�Ɍ������ēy�����グ��悤�ɂ���Bs�������Ȃ��ƌ����Ȃ�

    public float fireRate = 0.1f;   // ���ˊԊu
    public float damage = 10f;      // �_���[�W��
    public float range = 100f;      // Ray�̔򋗗�
    public float spread = 0.1f;     // �΂炯��͈�
    public VisualEffect effect;     // Hit���ɐ�����VFX

    public LineRenderer lineRenderer; // LineRenderer�R���|�[�l���g

    private float nextFireTime = 0f;

    private GraphicsBuffer positionBuffer;
    private GraphicsBuffer quaternionBuffer;

    private List<Vector3> positions = new();
    private List<Vector3> quaternions = new();

    private int particleCount = 0;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // ���˕����Ƀ����_���ȕ΍���������
        Vector3 shootDirection = transform.forward;
        shootDirection.x += Random.Range(-spread, spread);
        shootDirection.y += Random.Range(-spread, spread);
        shootDirection.z += Random.Range(-spread, spread);

        RaycastHit hit;
        Vector3 shootStart = effect.transform.position;
        Vector3 shootEnd = shootStart + shootDirection * range;

        if (Physics.Raycast(shootStart, shootDirection, out hit, range))
        {
            shootEnd = hit.point;

            // �@���x�N�g������]�ɕϊ����� (up������normal�ɂ���)
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // �N�H�[�^�j�I�����I�C���[�p�ɕϊ�
            Vector3 eulerAngles = rotation.eulerAngles;

            float[] targetAngles = { 0, 270, 90 };
            float closestAngle = targetAngles[0];
            float minDifference = float.MaxValue;

            for (int i = 0; i < targetAngles.Length; i++)
            {
                float difference = Mathf.Abs(Mathf.DeltaAngle(eulerAngles.x, targetAngles[i]));
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closestAngle = targetAngles[i];
                }
            }
            eulerAngles.x = closestAngle;

            closestAngle = targetAngles[0];
            minDifference = float.MaxValue;

            for (int i = 0; i < targetAngles.Length; i++)
            {
                float difference = Mathf.Abs(Mathf.DeltaAngle(eulerAngles.y, targetAngles[i]));
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closestAngle = targetAngles[i];
                }
            }
            eulerAngles.y = closestAngle;

            closestAngle = targetAngles[0];
            minDifference = float.MaxValue;

            for (int i = 0; i < targetAngles.Length; i++)
            {
                float difference = Mathf.Abs(Mathf.DeltaAngle(eulerAngles.z, targetAngles[i]));
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closestAngle = targetAngles[i];
                }
            }
            eulerAngles.z = closestAngle;

            // ��x�N�g����I�� (�Ⴆ�΁AZ������ɂ���)
            Vector3 reference = Vector3.back;

            // �N���X�ς��g�p���Ė@���ɐ����ȃx�N�g�����v�Z
            Vector3 perpendicularDirection = Vector3.Cross(hit.normal, reference);

            // �����v�Z�����x�N�g�����[���ɋ߂��i�@���Ɗ�x�N�g�������s�ȏꍇ�j�́A�ʂ̊�x�N�g�����g�p����
            if (perpendicularDirection.magnitude < 0.01f)
            {
                // X������ɍČv�Z
                perpendicularDirection = Vector3.Cross(hit.normal, Vector3.left);
            }

            // Hit����Mesh����hitPointOffset������������
            float hitPointOffset = 0.25f;    // �������鋗�����`
            Vector3 spawnPosition = hit.point + (perpendicularDirection * hitPointOffset);

            particleCount++;

            effect.SetUInt("SpawnCount", 1);

            positions.Add(spawnPosition);
            quaternions.Add(eulerAngles);

            // positionBuffer
            positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, positions.Count, sizeof(float) * 3);
            positionBuffer.SetData(positions);

            // quaternionBuffer
            quaternionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, quaternions.Count, sizeof(float) * 3);
            quaternionBuffer.SetData(quaternions);

            // GraphicsBuffer��VFX Graph�ɓn��
            effect.SetGraphicsBuffer("PositionBuffer", positionBuffer);
            effect.SetGraphicsBuffer("QuaternionBuffer", quaternionBuffer);

            effect.SendEvent("OnBulletHolePlay");

            StartCoroutine(Local(hit.transform, spawnPosition, eulerAngles, hit.normal, particleCount));
        }

        // �f�o�b�O�p�̒e����`��
        // �I�u�W�F�N�g�v�[��������
        GameObject lineObj = Instantiate(lineRenderer.gameObject);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        StartCoroutine(DrawLine(lr, transform.position, shootEnd));
    }

    private IEnumerator Local(Transform parent, Vector3 position, Vector3 angle, Vector3 normal, int particleCount)
    {
        Vector3 localPosition = parent.InverseTransformPoint(position);
        Quaternion localRotation = Quaternion.Euler(angle);

        float delta = 0.0f;

        while (delta < 1.0f)
        {
            delta += Time.deltaTime;
            yield return null;

            // �Ǐ]���̈ʒu�Ɗp�x
            Vector3 worldPosition = parent.TransformPoint(localPosition);
            Quaternion worldRotation = parent.rotation * localRotation;

            // Hit����Mesh����hitPointOffset������������
            Vector3 direction = worldPosition - parent.position;    // �������鋗�����`
            float hitPointOffset = 0.025f;                          // �������鋗����傫�����Ă��_�ł����B�����͌����ł͂Ȃ�
            Vector3 spawnPosition = worldPosition + (direction * hitPointOffset);

            // �O���菭�Ȃ��A��������葽���l������
            if (this.particleCount <= particleCount)
            {
                break;
            }

            positions[particleCount] = spawnPosition;
            quaternions[particleCount] = localRotation.eulerAngles;
        }

        particleCount--;
    }

    private IEnumerator DrawLine(LineRenderer lineRenderer, Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;

        // �����̃}�e���A������F���擾
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;

        float duration = 0.1f; // ����\�����鎞��
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // �A���t�@�l�����X�Ɍ���������
            float alpha = Mathf.Lerp(1f, 0f, t);

            // �V�����F��ݒ�
            lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lineRenderer.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

            yield return null;
        }

        // �Ō�Ɋ��S�ɔ�\���ɂ���
        //lineRenderer.enabled = false;

        Destroy(lineRenderer.gameObject);
    }
}
