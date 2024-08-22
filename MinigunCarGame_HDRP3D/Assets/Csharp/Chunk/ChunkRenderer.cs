using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

/// <summary>
/// チャンクのMesh、Collider関連のclass
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter _meshFilter;          // メッシュデータを保持するためのMeshFilterコンポーネント
    private MeshCollider _meshCollider;      // メッシュの衝突判定を行うためのMeshColliderコンポーネント
    private Mesh _mesh;                      // チャンクの形状データを保持するメッシュオブジェクト
    private NavMeshSurface surface = null;
    private bool _isInitialize = false;

    public bool showGizmo = false;  // シーンビューでGizmosを表示するかどうかのフラグ

    public ChunkInfo ChunkInfo { get; private set; } // チャンクに関連するデータを保持するプロパティ

    private void Awake()
    {
        // コンポーネントの参照を初期化する
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = _meshFilter.mesh;
    }

    // チャンクのデータを初期化するメソッド
    public void InitializeChunk(ChunkInfo data)
    {
        ChunkInfo = data;
        _isInitialize = true;

        if (surface != null && surface.TryGetComponent(out NavMeshSurface currentNavMeshSurface))
        {
            Destroy(currentNavMeshSurface);
        }

        GameObject surfaceObject = null;

        if (surface == null)
        {
            // NavMeshSurfaceの初期化
            surfaceObject = new GameObject();
        }
        else
        {
            surfaceObject = surface.gameObject;
        }
        surface = surfaceObject.AddComponent<NavMeshSurface>();

        Vector3 chunkCenter = new Vector3(ChunkInfo.BlocksXLength / 2, 0, ChunkInfo.BlocksZLength / 2);
        int chunkSize = Mathf.Max(ChunkInfo.BlocksXLength, ChunkInfo.BlocksZLength);

        surface.transform.SetParent(transform);
        surface.transform.localPosition = chunkCenter;

        surface.overrideTileSize = true;
        //surface.tileSize = chunkSize + 1;   // +1は重ねる為の値
        surface.tileSize = 1;   // +1は重ねる為の値
        surface.overrideVoxelSize = true;
        surface.voxelSize = 0.3f;

        if (surface.navMeshData == null)
        {
            surface.navMeshData = new UnityEngine.AI.NavMeshData();
        }
    }

    // メッシュデータを使用してチャンクのメッシュを描画するメソッド
    private void RenderMesh(ChunkMeshData meshData)
    {
        _mesh.Clear(); // 既存のメッシュデータをクリアする

        _mesh.subMeshCount = 2; // サブメッシュの数を2に設定（地面と水のメッシュ用）
        _mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray(); // 頂点データを設定

        // 三角形インデックスを設定（0が地面、1が水のメッシュ）
        // 地面のMesh
        _mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        // 水のMesh
        _mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1);

        _mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray(); // UV座標を設定
        _mesh.RecalculateNormals(); // 法線を再計算

        // VertexCountなどが 0の時に発生するエラーがあるので回避する
        if (meshData.colliderVertices.ToArray().Length == 0 || meshData.colliderTriangles.ToArray().Length == 0) { return; }

        // メッシュコライダーの設定
        _meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();   // 衝突判定用の頂点を設定
        collisionMesh.triangles = meshData.colliderTriangles.ToArray(); // 衝突判定用の三角形インデックスを設定
        collisionMesh.RecalculateNormals(); // 衝突判定用メッシュの法線を再計算

        _meshCollider.sharedMesh = collisionMesh; // コライダーにメッシュを適用
    }

    // 指定されたメッシュデータでチャンクを更新するメソッド
    public void UpdateChunk(ChunkMeshData data)
    {
        RenderMesh(data); // 渡されたメッシュデータを使用してチャンクを更新

        if (_isInitialize)
        {
            _isInitialize = false;

            surface.navMeshData = new NavMeshData();
            surface.BuildNavMesh();
        }
        else
        {
            surface.UpdateNavMesh(surface.navMeshData);
        }
    }

#if UNITY_EDITOR
    // エディターでGizmosを描画するメソッド（デバッグ用）
    private void OnDrawGizmos()
    {
        if (showGizmo) // Gizmosの表示フラグがtrueの場合
        {
            if (Application.isPlaying && ChunkInfo != null) // ゲームが実行中かつチャンクデータが存在する場合
            {
                if (Selection.activeObject == gameObject) // 選択されているオブジェクトがこのチャンクの場合
                    Gizmos.color = new Color(0, 1, 0, 0.4f); // 緑色で半透明のGizmosを描画
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f); // ピンク色で半透明のGizmosを描画

                // チャンクの範囲を示すGizmosを描画
                Gizmos.DrawCube(transform.position + new Vector3(ChunkInfo.BlocksXLength / 2f, ChunkInfo.BlocksYLength / 2f, ChunkInfo.BlocksZLength / 2f), new Vector3(ChunkInfo.BlocksXLength, ChunkInfo.BlocksYLength, ChunkInfo.BlocksZLength));
            }
        }
    }
#endif
}
