using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSingleton : MonoBehaviour
{
    private static GameManagerSingleton instance;

    void Awake()
    {
        // すでに存在していたら新しい方を破棄
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 最初のインスタンスを保持し、シーン切り替えで消えないようにする
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
