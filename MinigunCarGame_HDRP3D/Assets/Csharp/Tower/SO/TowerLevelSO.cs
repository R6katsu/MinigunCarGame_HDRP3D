using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タワーレベルごとの設定を保持するデータコンテナ
/// </summary>
[CreateAssetMenu(fileName = "TowerLevelSO", menuName = "ScriptableObject/TowerLevelSO")]
public class TowerLevelSO : ScriptableObject
{
    [SerializeField, Header("UIに表示するタワーの説明")]
    private string _description = "UIに表示するタワーの説明";

    [SerializeField, Header("UIに表示するタワーのアップグレード説明")]
    private string _upgradeDescription = "UIに表示するタワーのアップグレード説明";

    [SerializeField, Header("このレベルにアップグレードするためのコスト")]
    private int _cost = 0;

    [SerializeField, Header("タワーを売却する際の価格")]
    private int _sell = 0;

    [SerializeField, Min(0), Header("最大体力")]
    private int _maxHealth = 0;

    [SerializeField, Min(0), Header("初期体力")]
    private int _startingHealth = 0;

    [SerializeField, Header("タワーのアイコン")]
    private Sprite _icon = null;

    /// <summary>
    /// UIに表示するタワーの説明
    /// </summary>
    public string Description => _description;

    /// <summary>
    /// UIに表示するタワーのアップグレード説明
    /// </summary>
    public string UpgradeDescription => _upgradeDescription;

    /// <summary>
    /// このレベルにアップグレードするためのコスト
    /// </summary>
    public int Cost => _cost;

    /// <summary>
    /// タワーを売却する際の価格
    /// </summary>
    public int Sell => _sell;

    /// <summary>
    /// 最大体力
    /// </summary>
    public int MaxHealth => _maxHealth;

    /// <summary>
    /// 初期体力
    /// </summary>
    public int StartingHealth => _startingHealth;

    /// <summary>
    /// タワーのアイコン
    /// </summary>
    public Sprite Icon => _icon;
}

