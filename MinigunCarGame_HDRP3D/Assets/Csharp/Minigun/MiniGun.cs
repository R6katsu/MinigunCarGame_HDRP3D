using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MiniGun : MonoBehaviour
{
    // 地面に当たった時は上に向かって土埃を上げるようにする。sそうしないと見えない

    public float fireRate = 0.1f;   // 発射間隔
    public float damage = 10f;      // ダメージ量
    public float range = 100f;      // Rayの飛距離
    public float spread = 0.1f;     // ばらける範囲
    public VisualEffect effect;     // Hit時に生じるVFX

    public LineRenderer lineRenderer; // LineRendererコンポーネント

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
        // 発射方向にランダムな偏差を加える
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

            // 法線ベクトルを回転に変換する (up方向をnormalにする)
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // クォータニオンをオイラー角に変換
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

            // 基準ベクトルを選択 (例えば、Z軸を基準にする)
            Vector3 reference = Vector3.back;

            // クロス積を使用して法線に垂直なベクトルを計算
            Vector3 perpendicularDirection = Vector3.Cross(hit.normal, reference);

            // もし計算したベクトルがゼロに近い（法線と基準ベクトルが平行な場合）は、別の基準ベクトルを使用する
            if (perpendicularDirection.magnitude < 0.01f)
            {
                // X軸を基準に再計算
                perpendicularDirection = Vector3.Cross(hit.normal, Vector3.left);
            }

            // HitしたMeshからhitPointOffset距離浮かせる
            float hitPointOffset = 0.25f;    // 浮かせる距離を定義
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

            // GraphicsBufferをVFX Graphに渡す
            effect.SetGraphicsBuffer("PositionBuffer", positionBuffer);
            effect.SetGraphicsBuffer("QuaternionBuffer", quaternionBuffer);

            effect.SendEvent("OnBulletHolePlay");

            StartCoroutine(Local(hit.transform, spawnPosition, eulerAngles, hit.normal, particleCount));
        }

        // デバッグ用の弾道を描画
        // オブジェクトプールを作れよ
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

            // 追従時の位置と角度
            Vector3 worldPosition = parent.TransformPoint(localPosition);
            Quaternion worldRotation = parent.rotation * localRotation;

            // HitしたMeshからhitPointOffset距離浮かせる
            Vector3 direction = worldPosition - parent.position;    // 浮かせる距離を定義
            float hitPointOffset = 0.025f;                          // 浮かせる距離を大きくしても点滅した。ここは原因ではない
            Vector3 spawnPosition = worldPosition + (direction * hitPointOffset);

            // 前回より少ない、且つ総数より多い値だった
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

        // 既存のマテリアルから色を取得
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;

        float duration = 0.1f; // 線を表示する時間
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // アルファ値を徐々に減少させる
            float alpha = Mathf.Lerp(1f, 0f, t);

            // 新しい色を設定
            lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lineRenderer.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

            yield return null;
        }

        // 最後に完全に非表示にする
        //lineRenderer.enabled = false;

        Destroy(lineRenderer.gameObject);
    }
}
