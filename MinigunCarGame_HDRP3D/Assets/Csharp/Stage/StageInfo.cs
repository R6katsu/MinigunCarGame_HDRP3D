using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ情報
/// </summary>
[Serializable]
public class StageInfo
{
    [SerializeField, Header("ステージ名")]
    private string _stageName = "ステージ名";

    [SerializeField, Header("ステージ概要")]
    private string _stageSummary = "ステージ概要";

    [SerializeField, Header("読み込むシーン名")]
    private string _sceneName = "読み込むシーン名";

    /// <summary>
    /// ステージ名
    /// </summary>
    public string StageName => _stageName;

    /// <summary>
    /// ステージ概要
    /// </summary>
    public string StageSummary => _stageSummary;

    /// <summary>
    /// 読み込むシーン名
    /// </summary>
    public string SceneName => _sceneName;        // 全てのStageItemのsceneNameが存在しているかデバッグする機能を実装する
}
