using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

/// <summary>
/// �`�����N��Mesh�ACollider�֘A��class
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter _meshFilter;          // ���b�V���f�[�^��ێ����邽�߂�MeshFilter�R���|�[�l���g
    private MeshCollider _meshCollider;      // ���b�V���̏Փ˔�����s�����߂�MeshCollider�R���|�[�l���g
    private Mesh _mesh;                      // �`�����N�̌`��f�[�^��ێ����郁�b�V���I�u�W�F�N�g
    private NavMeshSurface surface = null;
    private bool _isInitialize = false;

    public bool showGizmo = false;  // �V�[���r���[��Gizmos��\�����邩�ǂ����̃t���O

    public ChunkInfo ChunkInfo { get; private set; } // �`�����N�Ɋ֘A����f�[�^��ێ�����v���p�e�B

    private void Awake()
    {
        // �R���|�[�l���g�̎Q�Ƃ�����������
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = _meshFilter.mesh;
    }

    // �`�����N�̃f�[�^�����������郁�\�b�h
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
            // NavMeshSurface�̏�����
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
        //surface.tileSize = chunkSize + 1;   // +1�͏d�˂�ׂ̒l
        surface.tileSize = 1;   // +1�͏d�˂�ׂ̒l
        surface.overrideVoxelSize = true;
        surface.voxelSize = 0.3f;

        if (surface.navMeshData == null)
        {
            surface.navMeshData = new UnityEngine.AI.NavMeshData();
        }
    }

    // ���b�V���f�[�^���g�p���ă`�����N�̃��b�V����`�悷�郁�\�b�h
    private void RenderMesh(ChunkMeshData meshData)
    {
        _mesh.Clear(); // �����̃��b�V���f�[�^���N���A����

        _mesh.subMeshCount = 2; // �T�u���b�V���̐���2�ɐݒ�i�n�ʂƐ��̃��b�V���p�j
        _mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray(); // ���_�f�[�^��ݒ�

        // �O�p�`�C���f�b�N�X��ݒ�i0���n�ʁA1�����̃��b�V���j
        // �n�ʂ�Mesh
        _mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        // ����Mesh
        _mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1);

        _mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray(); // UV���W��ݒ�
        _mesh.RecalculateNormals(); // �@�����Čv�Z

        // VertexCount�Ȃǂ� 0�̎��ɔ�������G���[������̂ŉ������
        if (meshData.colliderVertices.ToArray().Length == 0 || meshData.colliderTriangles.ToArray().Length == 0) { return; }

        // ���b�V���R���C�_�[�̐ݒ�
        _meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();   // �Փ˔���p�̒��_��ݒ�
        collisionMesh.triangles = meshData.colliderTriangles.ToArray(); // �Փ˔���p�̎O�p�`�C���f�b�N�X��ݒ�
        collisionMesh.RecalculateNormals(); // �Փ˔���p���b�V���̖@�����Čv�Z

        _meshCollider.sharedMesh = collisionMesh; // �R���C�_�[�Ƀ��b�V����K�p
    }

    // �w�肳�ꂽ���b�V���f�[�^�Ń`�����N���X�V���郁�\�b�h
    public void UpdateChunk(ChunkMeshData data)
    {
        RenderMesh(data); // �n���ꂽ���b�V���f�[�^���g�p���ă`�����N���X�V

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
    // �G�f�B�^�[��Gizmos��`�悷�郁�\�b�h�i�f�o�b�O�p�j
    private void OnDrawGizmos()
    {
        if (showGizmo) // Gizmos�̕\���t���O��true�̏ꍇ
        {
            if (Application.isPlaying && ChunkInfo != null) // �Q�[�������s�����`�����N�f�[�^�����݂���ꍇ
            {
                if (Selection.activeObject == gameObject) // �I������Ă���I�u�W�F�N�g�����̃`�����N�̏ꍇ
                    Gizmos.color = new Color(0, 1, 0, 0.4f); // �ΐF�Ŕ�������Gizmos��`��
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f); // �s���N�F�Ŕ�������Gizmos��`��

                // �`�����N�͈̔͂�����Gizmos��`��
                Gizmos.DrawCube(transform.position + new Vector3(ChunkInfo.BlocksXLength / 2f, ChunkInfo.BlocksYLength / 2f, ChunkInfo.BlocksZLength / 2f), new Vector3(ChunkInfo.BlocksXLength, ChunkInfo.BlocksYLength, ChunkInfo.BlocksZLength));
            }
        }
    }
#endif
}
