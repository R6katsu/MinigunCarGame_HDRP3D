using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シングルトン用のMonoBehaviour
/// </summary>
/// <typeparam name="T">シングルトン化する型</typeparam>
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
                    Debug.LogError($"現在のシーンに{t}をアタッチしているGameObjectはありません");
                }
            }
            return instance;
        }
    }
    virtual protected void Awake()
    {
        //他のゲームオブジェクトにアタッチされているか調べる
        //アタッチされている場合は破棄する
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
