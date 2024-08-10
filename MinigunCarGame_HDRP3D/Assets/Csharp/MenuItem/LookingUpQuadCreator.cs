#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// MenuItem���g���āA��������Ă���Quad�𐶐��ł���悤�ɂ���
/// </summary>
public class LookingUpQuadCreator
{
    private const int PRIORITY = 7; // �D��x��Quad�̎��ł��� 7�Ƃ���

    /// <summary>
    /// Prefab���瑊�΃p�X�Ŏ擾����LookingUpQuad�𐶐�����
    /// </summary>
    [MenuItem("GameObject/3D Object/LookingUpQuad", false, PRIORITY)]
    static void CreateLookingUpQuadPrefab()
    {
        // LookingUpQuad.prefab�̑��΃p�X���`
        string prefabPath = "Assets/Prefab/OriginalPrimitive/LookingUpQuad.prefab";

        // LookingUpQuad��Asset����擾
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        // null��������擾�ł��Ă��Ȃ�
        if (prefab == null)
        {
            Debug.LogError($"{prefabPath} ��������܂���ł���");
            return;
        }

        // �q�G�����L�[���Prefab���C���X�^���X��
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        // �C���X�^���X��I��
        Selection.activeGameObject = instance;

        // �ʒu�Ɖ�]�����Z�b�g
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
    }
}
#endif