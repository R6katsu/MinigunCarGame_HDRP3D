using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;

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

            Debug.Log("Hit: " + hit.transform.name);

            effect.SetUInt("SpawnCount", 10);
            effect.SetVector3("SpawnPosition", hit.point);
            effect.SendEvent("OnPlay");
        }

        // �f�o�b�O�p�̒e����`��
        // �I�u�W�F�N�g�v�[��������
        GameObject lineObj = Instantiate(lineRenderer.gameObject);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        StartCoroutine(DrawLine(lr, transform.position, shootEnd));
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
        lineRenderer.enabled = false;
    }
}
