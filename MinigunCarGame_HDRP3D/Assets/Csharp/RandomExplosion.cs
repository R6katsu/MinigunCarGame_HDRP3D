using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RandomExplosion : MonoBehaviour
{
    [SerializeField]
    private NavMeshEnemy[] enemyPrefabs = null;

    [SerializeField]
    private Transform target = null;

    private void OnEnable()
    {
        InvokeRepeating("AHFUAIOJ", 0.0f, 0.5f);
    }

    private void AHFUAIOJ()
    {
        NavMeshEnemy randomNavMeshEnemy = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];

        GameObject enemyObject = GenerateWithinRange.GenerateAtRandomDistance(randomNavMeshEnemy.gameObject, Vector3.up * 10, 30, 30);
        randomNavMeshEnemy.target = target;

        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-30.0f, 30.0f), 13, UnityEngine.Random.Range(-30.0f, 30.0f));
        float randomRadius = UnityEngine.Random.Range(0.5f, 3.0f);

        /*
        Explosion.ExplosionAAA(randomPosition, randomRadius, new(1));

        NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
        surface.UpdateNavMesh(surface.navMeshData);
        */
    }
}
