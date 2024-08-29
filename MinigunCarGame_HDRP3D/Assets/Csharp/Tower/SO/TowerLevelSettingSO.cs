using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タワーレベルの設定
/// </summary>
[CreateAssetMenu(fileName = "TowerLevelSettingSO", menuName = "ScriptableObject/TowerLevelSettingSO")]
public class TowerLevelSettingSO : ScriptableObject
{
    [SerializeField, Header("タワーの説明")]
    private string _description = "タワーの説明";

    [SerializeField, Header("タワーのアップグレード説明")]
    private string _upgradeDescription = "タワーのアップグレード説明";

    [SerializeField, Header("アップグレードに必要なコスト")]
    private int _cost = 0;

    [SerializeField, Header("タワーを売却価格")]
    private int _sell = 0;

    [SerializeField, Min(0), Header("最大体力")]
    private int _maxHealth = 0;

    [SerializeField, Min(0), Header("初期体力")]
    private int _startingHealth = 0;

    [SerializeField, Header("タワーのアイコン")]
    private Sprite _icon = null;

    /// <summary>
    /// タワーの説明
    /// </summary>
    public string Description => _description;

    /// <summary>
    /// タワーのアップグレード説明
    /// </summary>
    public string UpgradeDescription => _upgradeDescription;

    /// <summary>
    /// アップグレードに必要なコスト
    /// </summary>
    public int Cost => _cost;

    /// <summary>
    /// タワーを売却価格
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

