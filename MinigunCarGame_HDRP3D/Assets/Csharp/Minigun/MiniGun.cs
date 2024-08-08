using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGun : MonoBehaviour
{
    // Rayを飛ばし、当たった敵にダメージを与える。
    // 弾にRBはアタッチせず、弾道にエフェクトのみを描写する。
    // 弾は落ちず、偏差も一切考慮する必要がない。
    // ミニガンである為、かなり弾はばらけると思われる

    public float fireRate = 0.1f; // 発射間隔
    public float damage = 10f; // ダメージ量
    public float range = 100f; // Rayの飛距離
    public float spread = 0.1f; // ばらける範囲

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

        RaycastHit hit;
        Vector3 shootEnd = transform.position + shootDirection * range;

        if (Physics.Raycast(transform.position, shootDirection, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
        }

        // デバッグ用の弾道を描画
        // オブジェクトプールを作れよ
        GameObject lineObj = Instantiate(lineRenderer.gameObject);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        StartCoroutine(DrawLine(lr, transform.position, shootEnd));
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
        lineRenderer.enabled = false;
    }
}
