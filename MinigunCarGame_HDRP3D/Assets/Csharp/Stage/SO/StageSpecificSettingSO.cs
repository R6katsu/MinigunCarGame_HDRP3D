using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージごとの設定
/// </summary>
[CreateAssetMenu(fileName = "StageSpecificSettingSO", menuName = "ScriptableObject/StageSpecificSettingSO")]
public class StageSpecificSettingSO : ScriptableObject, IDictionary<string, StageInfo>, IList<StageInfo>
{
    public StageInfo[] levels;

    /// <summary>
    /// IDによってキャッシュされたレベルの辞書
    /// </summary>
    IDictionary<string, StageInfo> m_LevelDictionary;

    /// <summary>
    /// レベルの数を取得します
    /// </summary>
    public int Count
    {
        get { return levels.Length; }
    }

    /// <summary>
    /// レベルリストは常に読み取り専用です
    /// </summary>
    public bool IsReadOnly
    {
        get { return true; }
    }

    /// <summary>
    /// インデックスでレベルを取得します
    /// </summary>
    public StageInfo this[int i]
    {
        get { return levels[i]; }
    }

    /// <summary>
    /// IDでレベルを取得します
    /// </summary>
    public StageInfo this[string key]
    {
        get { return m_LevelDictionary[key]; }
    }

    /// <summary>
    /// すべてのレベルのキーのコレクションを取得します
    /// </summary>
    public ICollection<string> Keys
    {
        get { return m_LevelDictionary.Keys; }
    }

    /// <summary>
    /// 指定されたレベルのインデックスを取得します
    /// </summary>
    public int IndexOf(StageInfo item)
    {
        if (item == null)
        {
            return -1;
        }

        for (int i = 0; i < levels.Length; ++i)
        {
            if (levels[i] == item)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// このリストにレベルが存在するかを確認します
    /// </summary>
    public bool Contains(StageInfo item)
    {
        return IndexOf(item) >= 0;
    }

    /// <summary>
    /// 指定されたIDのレベルが存在するかを確認します
    /// </summary>
    public bool ContainsKey(string key)
    {
        return m_LevelDictionary.ContainsKey(key);
    }

    /// <summary>
    /// 指定されたキーでレベルを取得しようとします
    /// </summary>
    public bool TryGetValue(string key, out StageInfo value)
    {
        return m_LevelDictionary.TryGetValue(key, out value);
    }

    /// <summary>
    /// 指定されたシーンに関連する<see cref="StageInfo"/>を取得します
    /// </summary>
    public StageInfo GetLevelByScene(string scene)
    {
        for (int i = 0; i < levels.Length; ++i)
        {
            StageInfo item = levels[i];
            if (item != null &&
                item.SceneName == scene)
            {
                return item;
            }
        }

        return null;
    }


    ICollection<StageInfo> IDictionary<string, StageInfo>.Values
    {
        get { return m_LevelDictionary.Values; }
    }

    StageInfo IList<StageInfo>.this[int i]
    {
        get { return levels[i]; }
        set { throw new NotSupportedException("Level List is read only"); }
    }

    StageInfo IDictionary<string, StageInfo>.this[string key]
    {
        get { return m_LevelDictionary[key]; }
        set { throw new NotSupportedException("Level List is read only"); }
    }

    void IList<StageInfo>.Insert(int index, StageInfo item)
    {
        throw new NotSupportedException("Level List is read only");
    }

    void IList<StageInfo>.RemoveAt(int index)
    {
        throw new NotSupportedException("Level List is read only");
    }

    void ICollection<StageInfo>.Add(StageInfo item)
    {
        throw new NotSupportedException("Level List is read only");
    }

    void ICollection<KeyValuePair<string, StageInfo>>.Add(KeyValuePair<string, StageInfo> item)
    {
        throw new NotSupportedException("Level List is read only");
    }

    void ICollection<KeyValuePair<string, StageInfo>>.Clear()
    {
        throw new NotSupportedException("Level List is read only");
    }

    bool ICollection<KeyValuePair<string, StageInfo>>.Contains(KeyValuePair<string, StageInfo> item)
    {
        return m_LevelDictionary.Contains(item);
    }

    void ICollection<KeyValuePair<string, StageInfo>>.CopyTo(KeyValuePair<string, StageInfo>[] array, int arrayIndex)
    {
        m_LevelDictionary.CopyTo(array, arrayIndex);
    }

    void ICollection<StageInfo>.Clear()
    {
        throw new NotSupportedException("Level List is read only");
    }

    void ICollection<StageInfo>.CopyTo(StageInfo[] array, int arrayIndex)
    {
        levels.CopyTo(array, arrayIndex);
    }

    bool ICollection<StageInfo>.Remove(StageInfo item)
    {
        throw new NotSupportedException("Level List is read only");
    }

    public IEnumerator<StageInfo> GetEnumerator()
    {
        return ((IList<StageInfo>)levels).GetEnumerator();
    }

    IEnumerator<KeyValuePair<string, StageInfo>> IEnumerable<KeyValuePair<string, StageInfo>>.GetEnumerator()
    {
        return m_LevelDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return levels.GetEnumerator();
    }

    void IDictionary<string, StageInfo>.Add(string key, StageInfo value)
    {
        throw new NotSupportedException("Level List is read only");
    }

    bool ICollection<KeyValuePair<string, StageInfo>>.Remove(KeyValuePair<string, StageInfo> item)
    {
        throw new NotSupportedException("Level List is read only");
    }

    bool IDictionary<string, StageInfo>.Remove(string key)
    {
        throw new NotSupportedException("Level List is read only");
    }
}
