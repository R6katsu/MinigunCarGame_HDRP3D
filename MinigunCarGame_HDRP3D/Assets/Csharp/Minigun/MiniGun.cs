using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MiniGun : MonoBehaviour
{
    public float fireRate = 0.1f;   // 発射間隔
    public float damage = 10f;      // ダメージ量
    public float range = 100f;      // Rayの飛距離
    public float spread = 0.1f;     // ばらける範囲
    public VisualEffect effect;     // Hit時に生じるVFX

    public LineRenderer lineRenderer; // LineRendererコンポーネント

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
        // 発射方向にランダムな偏差を加える
        Vector3 shootDirection = transform.forward;
        shootDirection.x += Random.Range(-spread, spread);
        shootDirection.y += Random.Range(-spread, spread);
        shootDirection.z += Random.Range(-spread, spread);

        RaycastHit hit;
        Vector3 shootEnd = transform.position + shootDirection * range;

        if (Physics.Raycast(transform.position, shootDirection, out hit, range))
        {
            shootEnd = hit.point;

            // 地面にパーティクルが生成された時に気持ち悪く点滅している
            // Wallの側面にパーティクルが生成された時、上を向いてしまっている
            // 回転させる軸が違うのかもしれない

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

            // スマートではないものの、一応全面に角度を対応させた筈
            // Meshに埋まっているので、次は浮かせる方向を整える
            /*
            // 上
            if (Mathf.RoundToInt(eulerAngles.x) == 0 && Mathf.RoundToInt(eulerAngles.y) == 0 && Mathf.RoundToInt(eulerAngles.z) == 0)
            {
                eulerAngles.x += 90.0f;
            }
            // 下
            else if (Mathf.RoundToInt(eulerAngles.y) == 180 && Mathf.RoundToInt(eulerAngles.z) == 180)
            {
                eulerAngles.x -= 90;
            }
            // 正面
            else if (Mathf.RoundToInt(eulerAngles.x) == 270)
            {
                eulerAngles.x += 90.0f;
            }
            // 左側面
            else if(Mathf.RoundToInt(eulerAngles.z) == 90)
            {
                eulerAngles.y += 90.0f;
            }
            // 右側面
            else if(Mathf.RoundToInt(eulerAngles.z) == 270)
            {
                eulerAngles.y -= 90.0f;
            }
            // 背後
            else if(Mathf.RoundToInt(eulerAngles.x) == 90)
            {
                eulerAngles.x += 90.0f;
            }
            */

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

            VisualEffect tempEffect = Instantiate(effect, transform.position, Quaternion.identity);
            tempEffect.transform.parent = transform;
            tempEffect.transform.localPosition = Vector3.up;

            // 実際に動かしてみると上手くいかない
            // そもそも斜めに対応していないっぽい
            // あとパーティクルが点滅したり見えない時があって違和感がある
            // 地面に生成されたパーティクルは特に点滅が酷い。カメラの角度的に平たいパーティクルは点滅する？
            tempEffect.SetUInt("SpawnCount", 1);

            // 生成時の位置と角度
            tempEffect.SetVector3("SpawnPosition", spawnPosition);
            tempEffect.SetVector3("SpawnAngle", eulerAngles);

            tempEffect.SendEvent("OnPlay");

            StartCoroutine(Local(hit.transform, spawnPosition, eulerAngles, hit.normal, tempEffect));
        }

        // デバッグ用の弾道を描画
        // オブジェクトプールを作れよ
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

            // 追従時の位置と角度
            Vector3 worldPosition = parent.TransformPoint(localPosition);
            Quaternion worldRotation = parent.rotation * localRotation;

            // HitしたMeshからhitPointOffset距離浮かせる
            Vector3 direction = worldPosition - parent.position;    // 浮かせる距離を定義
            float hitPointOffset = 0.025f;                          // 浮かせる距離を大きくしても点滅した。ここは原因ではない
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
