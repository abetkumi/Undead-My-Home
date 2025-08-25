using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //ゲームの状態
    public enum GameState
    {
        enGameState_Play,
        enGameState_Clear,
    }
    static GameState m_gameState = GameState.enGameState_Play;

    public void SetGameState(GameState gameState)
    {
        m_gameState = gameState;
    }
    //どこからでも呼び出せる関数
    public static GameState GetGameState()
    {
        return m_gameState;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ステートの更新（初期化）
        m_gameState = GameState.enGameState_Play;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
