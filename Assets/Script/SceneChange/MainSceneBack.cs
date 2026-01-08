using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneBack : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_gameClearObject;
    [SerializeField] GameObject m_gameOverObject;
    GameManager m_gameManager;

    bool m_isInArea = false;
    int m_clearCount = 0;
    private void Awake()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

        m_clearCount = m_gameManager.GetClearCount();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isInArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isInArea = false;
        }
    }


    void SetMainGameBack()
    {
        // シーン切替
        // フェード演出用オブジェクトを生成
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // 生成したオブジェクトのFadeStart関数を呼び出す
        fadeObject.GetComponent<FadeScene>().FadeStart("MainGameScene", Color.black, true);
        UI_Timer m_timer = GameObject.FindWithTag("Timer").GetComponent<UI_Timer>();
        m_timer.ResetTimer();
        
        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!m_isInArea)
        {
            return;
        }

        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
                "1日を終了する", true);

        if (Input.GetKeyDown("joystick button 0") || Input.GetMouseButtonDown(0))
        {

            int clearCountNow = m_gameManager.GetClearCount();
            //ノルマをすべて達成したのでゲームクリア
            if (clearCountNow >= m_gameManager.GetClearCondition())
            {
                GameClear gameClear = m_gameClearObject.GetComponent<GameClear>();
                gameClear.SetGameClear();
                Debug.Log("Clear");
            }
            //ノルマ達成のため次のゲームへ
            else if (m_clearCount != clearCountNow)
            {
                SetMainGameBack();
            }
            //ノルマ未達成のためゲームオーバー
            else
            {
                Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
                player.SetPlayerState(Player.PlayerState.Dead);
            }
        }
    }
}
