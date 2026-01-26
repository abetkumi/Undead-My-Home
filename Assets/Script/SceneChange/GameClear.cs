using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] ItemData Item_Data;
    GameManager m_gameManager;
    UI_Timer m_timer;

    bool m_isArea = false;

    private void Awake()
    {
        //ゲームマネージャーを取得
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //ライトスクリプトを取得
        m_timer = GameObject.FindWithTag("Timer").GetComponent<UI_Timer>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_B,
                "探索を終了する", true);
            m_isArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isArea = false;
        }
    }

    void SetStoreScene()
    {
        // シーン切替
        // フェード演出用オブジェクトを生成
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // 生成したオブジェクトのFadeStart関数を呼び出す
        fadeObject.GetComponent<FadeScene>().FadeStart("StoreScene", Color.black, true);
        m_timer.ResetTimer();
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
        GameObject item = GameObject.FindWithTag("Item");
        if (item != null)
        {
            Destroy(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isArea == false)
        {
            return;
        }
        if (Input.GetKeyDown("joystick button 0") || Input.GetMouseButtonDown(0))
        {
            SetStoreScene();
            m_isArea = false;
            Debug.Log("ショップへ");
        }
    }
}
