using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Explosion
{
    static public void ExplosionAAA(Vector3 position, float radius, AttackInfo attackInfo = new())
    {
        // 爆発範囲内のColを配列で取得
        var hitColliders = Physics.OverlapSphere(position, radius);

        foreach (var hit in hitColliders)
        {
            // 体力が実装されていた
            if (hit.TryGetComponent(out IHealth health))
            {
                health.GetHealth().Damage(attackInfo);
            }

            // Terrainだった
            if (hit.TryGetComponent(out Terrain terrain))
            {
                TerrainChange.ModifyTerrain(terrain, position, radius, 0.025f);     // 0.025fは仮
            }
        }
    }
}
