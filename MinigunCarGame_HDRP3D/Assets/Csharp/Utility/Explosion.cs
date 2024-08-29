using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Explosion
{
    static public void ExplosionAAA(Vector3 position, float radius, AttackInfo attackInfo = new())
    {
        // ”š”­”ÍˆÍ“à‚ÌCol‚ð”z—ñ‚ÅŽæ“¾
        var hitColliders = Physics.OverlapSphere(position, radius);

        foreach (var hit in hitColliders)
        {
            // ‘Ì—Í‚ªŽÀ‘•‚³‚ê‚Ä‚¢‚½
            if (hit.TryGetComponent(out IHealth health))
            {
                health.GetHealth().Damage(attackInfo);
            }

            // Terrain‚¾‚Á‚½
            if (hit.TryGetComponent(out Terrain terrain))
            {
                TerrainChange.ModifyTerrain(terrain, position, radius, 0.025f);     // 0.025f‚Í‰¼
            }
        }
    }
}
