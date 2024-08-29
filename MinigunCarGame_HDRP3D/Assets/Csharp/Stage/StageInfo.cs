using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W���
/// </summary>
[Serializable]
public class StageInfo
{
    [SerializeField, Header("�X�e�[�W��")]
    private string _stageName = "�X�e�[�W��";

    [SerializeField, Header("�X�e�[�W�T�v")]
    private string _stageSummary = "�X�e�[�W�T�v";

    [SerializeField, Header("�ǂݍ��ރV�[����")]
    private string _sceneName = "�ǂݍ��ރV�[����";

    /// <summary>
    /// �X�e�[�W��
    /// </summary>
    public string StageName => _stageName;

    /// <summary>
    /// �X�e�[�W�T�v
    /// </summary>
    public string StageSummary => _stageSummary;

    /// <summary>
    /// �ǂݍ��ރV�[����
    /// </summary>
    public string SceneName => _sceneName;        // �S�Ă�StageItem��sceneName�����݂��Ă��邩�f�o�b�O����@�\����������
}
