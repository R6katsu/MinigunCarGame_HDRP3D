using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockType;

/// <summary>
/// �`�����N��Mesh���쐬����ׂ̕⏕�I��class�B<br/>
/// �`�����N���̑S�Ẵu���b�N��Mesh��ێ�������A�p������Quad�̍쐬��⏕����B
/// </summary>
public class ChunkMeshData
{
    public List<Vector3> vertices = new List<Vector3>();    // Mesh���쐬����ׂ̒��_���X�g
    public List<int> triangles = new List<int>();           // Mesh���쐬����ׂ̃|���S�����X�g
    public List<Vector2> uv = new List<Vector2>();          // Mesh���쐬����ׂ�UV���X�g

    // �t�̂��C�i�S�Ă̓��߃u���b�N������Ȃ���ł͂Ȃ��j�͗e��Ȃ��A�t���ϓ��͗e���
    public List<Vector3> colliderVertices = new List<Vector3>();    // Collider��K�p����Mesh�̒��_���X�g
    public List<int> colliderTriangles = new List<int>();           // Collider��K�p����Mesh�̃|���S�����X�g

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
    /// �w�肳�ꂽ�����ɑ΂���ʂ̒��_���擾����B<br/>
    /// �O�Ȃ�O������Quad�쐬����B�쐬����Quad�̓u���b�N��Mesh�ƂȂ�A�u���b�N��Mesh�̓`�����N��Mesh�ɓ��������
    /// </summary>
    /// <param name="direction">�ʂ̕����B</param>
    /// <param name="x">�u���b�N�� x ���W�B</param>
    /// <param name="y">�u���b�N�� y ���W�B</param>
    /// <param name="z">�u���b�N�� z ���W�B</param>
    /// <param name="meshData">��������郁�b�V���f�[�^�B</param>
    /// <param name="blockType">��������u���b�N�̎�ށB</param>
    public static void GetFaceVertices(Direction direction, int x, int y, int z, ChunkMeshData meshData, BlockNameType blockType)
    {
        // �u���b�N�̃e�N�X�`���Ɋ֘A����f�[�^�̊Ǘ����W�������A
        // ���̕����̃X�N���v�g�������̃f�[�^�ɊȒP�ɃA�N�Z�X�ł���悤�ɂ���

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

        // ���_�̏����͖@���̌v�Z�⃁�b�V���̕`��ɉe����^����
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
