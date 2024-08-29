using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Explosion
{
    static public void ExplosionAAA(Vector3 position, float radius, AttackInfo attackInfo = new())
    {
        // �����͈͓���Col��z��Ŏ擾
        var hitColliders = Physics.OverlapSphere(position, radius);

        foreach (var hit in hitColliders)
        {
            // �̗͂���������Ă���
            if (hit.TryGetComponent(out IHealth health))
            {
                health.GetHealth().Damage(attackInfo);
            }

            // Terrain������
            if (hit.TryGetComponent(out Terrain terrain))
            {
                TerrainChange.ModifyTerrain(terrain, position, radius, 0.025f);     // 0.025f�͉�
            }
        }
    }
}
