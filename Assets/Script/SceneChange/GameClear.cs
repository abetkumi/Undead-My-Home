using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    GameManager m_gameManager;
    LightONOFF m_lightScript;
    //GameObject m_playerObject;

    bool m_isGameClaer = false;

    private void Awake()
    {
        //ゲームマネージャーを取得
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //プレイヤーを取得
        //m_playerObject = GameObject.FindGameObjectWithTag("Player");
        //ライトスクリプトを取得
        m_lightScript = GameObject.FindWithTag("Player").GetComponent<LightONOFF>();
    }
    private void OnTriggerStay(Collider other)
    {
        m_lightScript.m_isActionFlag = false;
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_X,
                "探索を終了する", true);
        if (other.CompareTag("Player") && Input.GetButtonUp("Action"))
        {
            int clearCountNow = m_gameManager.GetClearCount();
            if(clearCountNow == 3)
            {
                SetGameClear();

                Debug.Log("Clear");
            }
            else
            {
                SetStoreScene(); 
                clearCountNow++;
                m_gameManager.SetClearCount(clearCountNow);
                m_lightScript.m_isActionFlag = true;
                Debug.Log("ショップへ");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_lightScript.m_isActionFlag = true;
        }
    }

    void SetStoreScene()
    {
        // シーン切替
        // フェード演出用オブジェクトを生成
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // 生成したオブジェクトのFadeStart関数を呼び出す
        fadeObject.GetComponent<FadeScene>().FadeStart("StoreScene", Color.black, true);

        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(fadeObject);
    }
    async public void SetGameClear()
    {
        m_gameManager.SetGameState(GameManager.GameState.enGameState_GameClear);

        await UniTask.Delay(1000);
        // シーン切替
        // フェード演出用オブジェクトを生成
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // 生成したオブジェクトのFadeStart関数を呼び出す
        fadeObject.GetComponent<FadeScene>().FadeStart("GameClearScene", Color.black, true);

        m_isGameClaer = true;
        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isGameClaer == false)
        {
            return;
        }

        if (Input.anyKeyDown)
        {
            GameObject fadeObject = Instantiate(m_fadeCanvas);
            fadeObject.GetComponent<FadeScene>().FadeStart("TitleScene", Color.black, true);
            Destroy(gameObject);
        }
    }
}
