#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// MenuItemを使って、上を向いているQuadを生成できるようにする
/// </summary>
public class LookingUpQuadCreator
{
    private const int PRIORITY = 7; // 優先度をQuadの次である 7とする

    /// <summary>
    /// Prefabから相対パスで取得したLookingUpQuadを生成する
    /// </summary>
    [MenuItem("GameObject/3D Object/LookingUpQuad", false, PRIORITY)]
    static void CreateLookingUpQuadPrefab()
    {
        // LookingUpQuad.prefabの相対パスを定義
        string prefabPath = "Assets/Prefab/OriginalPrimitive/LookingUpQuad.prefab";

        // LookingUpQuadをAssetから取得
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        // nullだったら取得できていない
        if (prefab == null)
        {
            Debug.LogError($"{prefabPath} が見つかりませんでした");
            return;
        }

        // ヒエラルキー上にPrefabをインスタンス化
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        // インスタンスを選択
        Selection.activeGameObject = instance;

        // 位置と回転をリセット
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
    }
}
#endif