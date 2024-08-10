using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MiniGun : MonoBehaviour
{
    public float fireRate = 0.1f;   // ���ˊԊu
    public float damage = 10f;      // �_���[�W��
    public float range = 100f;      // Ray�̔򋗗�
    public float spread = 0.1f;     // �΂炯��͈�
    public VisualEffect effect;     // Hit���ɐ�����VFX

    public LineRenderer lineRenderer; // LineRenderer�R���|�[�l���g

    private float nextFireTime = 0f;

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
        Vector3 shootEnd = transform.position + shootDirection * range;

        if (Physics.Raycast(transform.position, shootDirection, out hit, range))
        {
            shootEnd = hit.point;

            // �n�ʂɃp�[�e�B�N�����������ꂽ���ɋC���������_�ł��Ă���
            // Wall�̑��ʂɃp�[�e�B�N�����������ꂽ���A��������Ă��܂��Ă���
            // ��]�����鎲���Ⴄ�̂�������Ȃ�

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

            // �X�}�[�g�ł͂Ȃ����̂́A�ꉞ�S�ʂɊp�x��Ή���������
            // Mesh�ɖ��܂��Ă���̂ŁA���͕�����������𐮂���
            /*
            // ��
            if (Mathf.RoundToInt(eulerAngles.x) == 0 && Mathf.RoundToInt(eulerAngles.y) == 0 && Mathf.RoundToInt(eulerAngles.z) == 0)
            {
                eulerAngles.x += 90.0f;
            }
            // ��
            else if (Mathf.RoundToInt(eulerAngles.y) == 180 && Mathf.RoundToInt(eulerAngles.z) == 180)
            {
                eulerAngles.x -= 90;
            }
            // ����
            else if (Mathf.RoundToInt(eulerAngles.x) == 270)
            {
                eulerAngles.x += 90.0f;
            }
            // ������
            else if(Mathf.RoundToInt(eulerAngles.z) == 90)
            {
                eulerAngles.y += 90.0f;
            }
            // �E����
            else if(Mathf.RoundToInt(eulerAngles.z) == 270)
            {
                eulerAngles.y -= 90.0f;
            }
            // �w��
            else if(Mathf.RoundToInt(eulerAngles.x) == 90)
            {
                eulerAngles.x += 90.0f;
            }
            */

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

            VisualEffect tempEffect = Instantiate(effect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = transform;
            tempEffect.transform.localPosition = Vector3.up;

            // ���ۂɓ������Ă݂�Ə�肭�����Ȃ�
            // ���������΂߂ɑΉ����Ă��Ȃ����ۂ�
            // ���ƃp�[�e�B�N�����_�ł����茩���Ȃ����������Ĉ�a��������
            // �n�ʂɐ������ꂽ�p�[�e�B�N���͓��ɓ_�ł������B�J�����̊p�x�I�ɕ������p�[�e�B�N���͓_�ł���H
            tempEffect.SetUInt("SpawnCount", 1);

            // �������̈ʒu�Ɗp�x
            tempEffect.SetVector3("SpawnPosition", spawnPosition);
            tempEffect.SetVector3("SpawnAngle", eulerAngles);

            tempEffect.SendEvent("OnPlay");

            StartCoroutine(Local(hit.transform, spawnPosition, eulerAngles, hit.normal, tempEffect));
        }

        // �f�o�b�O�p�̒e����`��
        // �I�u�W�F�N�g�v�[��������
        GameObject lineObj = Instantiate(lineRenderer.gameObject);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        StartCoroutine(DrawLine(lr, transform.position, shootEnd));
    }

    private IEnumerator Local(Transform parent, Vector3 position, Vector3 angle, Vector3 normal, VisualEffect tempEffect)
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

            tempEffect.SetVector3("LocalPosition", spawnPosition);
            tempEffect.SetVector3("LocalAngle", localRotation.eulerAngles);
        }

        Destroy(tempEffect.gameObject);
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
