using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �V���O���g���p��MonoBehaviour
/// </summary>
/// <typeparam name="T">�V���O���g��������^</typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);

                if (instance == null)
                {
                    Debug.LogError($"���݂̃V�[����{t}���A�^�b�`���Ă���GameObject�͂���܂���");
                }
            }
            return instance;
        }
    }
    virtual protected void Awake()
    {
        //���̃Q�[���I�u�W�F�N�g�ɃA�^�b�`����Ă��邩���ׂ�
        //�A�^�b�`����Ă���ꍇ�͔j������
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        else if (instance == this)
        {
            return true;
        }
        Destroy(this);
        return false;
    }
}
