using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockType;

/// <summary>
/// チャンクのMeshを作成する為の補助的なclass。<br/>
/// チャンク内の全てのブロックのMeshを保持したり、角方向のQuadの作成を補助する。
/// </summary>
public class ChunkMeshData
{
    public List<Vector3> vertices = new List<Vector3>();    // Meshを作成する為の頂点リスト
    public List<int> triangles = new List<int>();           // Meshを作成する為のポリゴンリスト
    public List<Vector2> uv = new List<Vector2>();          // Meshを作成する為のUVリスト

    // 液体や空気（全ての透過ブロックが入らない訳ではない）は容れない、葉っぱ等は容れる
    public List<Vector3> colliderVertices = new List<Vector3>();    // Colliderを適用するMeshの頂点リスト
    public List<int> colliderTriangles = new List<int>();           // Colliderを適用するMeshのポリゴンリスト

    public ChunkMeshData waterMesh;
    private bool isMainMesh = true;

    public ChunkMeshData(bool isMainMesh)
    {
        if (isMainMesh)
        {
            waterMesh = new ChunkMeshData(false);
        }
    }

    public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider)
    {
        vertices.Add(vertex);
        if (vertexGeneratesCollider)
        {
            colliderVertices.Add(vertex);
        }

    }

    public void AddQuadTriangles(bool quadGeneratesCollider)
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        if (quadGeneratesCollider)
        {
            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 3);
            colliderTriangles.Add(colliderVertices.Count - 2);
            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 2);
            colliderTriangles.Add(colliderVertices.Count - 1);
        }
    }

    /// <summary>
    /// 指定された方向に対する面の頂点を取得する。<br/>
    /// 前なら前方向にQuad作成する。作成したQuadはブロックのMeshとなり、ブロックのMeshはチャンクのMeshに統合される
    /// </summary>
    /// <param name="direction">面の方向。</param>
    /// <param name="x">ブロックの x 座標。</param>
    /// <param name="y">ブロックの y 座標。</param>
    /// <param name="z">ブロックの z 座標。</param>
    /// <param name="meshData">生成されるメッシュデータ。</param>
    /// <param name="blockType">処理するブロックの種類。</param>
    public static void GetFaceVertices(Direction direction, int x, int y, int z, ChunkMeshData meshData, BlockNameType blockType)
    {
        // ブロックのテクスチャに関連するデータの管理を集中化し、
        // 他の部分のスクリプトがこれらのデータに簡単にアクセスできるようにする

        bool generatesCollider;

        switch (GetBlockBehaviourType(blockType))
        {
            case BlockBehaviourType.Gas:
            case BlockBehaviourType.Liquid:
                generatesCollider = false;
                break;

            default:
                generatesCollider = true;
                break;
        }

        // 頂点の順序は法線の計算やメッシュの描画に影響を与える
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.foreward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }
}
