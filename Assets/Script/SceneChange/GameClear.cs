using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] ItemData Item_Data;
    GameManager m_gameManager;
    UI_Timer m_timer;

    bool m_isArea = false;
    public bool m_isWait = false;

    private void Awake()
    {
        //ゲームマネージャーを取得
        m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //ライトスクリプトを取得
        m_timer = GameObject.FindWithTag("Timer").GetComponent<UI_Timer>();
        m_isArea = false;
        m_isWait = false;
    }

    //プレイヤーが出口判定に入った時
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_B,
                "探索を終了する", true);
            m_isArea = true;
        }
    }

    //プレイヤーが出口判定から出た時
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_B,
                "", true);
            m_isArea = false;
            m_isWait = false;
        }
    }

    public void SetStoreScene()
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


        //シーン移行時にプレイヤーを削除できるように変更
        GameObject m_playerParentObject = GameObject.FindWithTag("PlayerParent");
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(m_playerParentObject, activeScene);

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

    //メインシーンから移動するかのUIを表示
    void WaitStoreScene()
    {
        UI_Caution cautionUI = GameObject.FindWithTag("Caution").GetComponent<UI_Caution>();
        if (cautionUI != null)
        {
            GameManager gameManager = GameObject.FindWithTag("GameController").GetComponentInParent<GameManager>();
            gameManager.SetGameState(GameManager.GameState.enGameState_Pause);
            cautionUI = GameObject.FindWithTag("Caution").GetComponent<UI_Caution>();
            cautionUI.SetActiveCautionUI(true);
            cautionUI.SetCautionText("探索を終了しますか？");
            cautionUI.SetYesButton(0);
            cautionUI.m_yesButton.Select();
            Time.timeScale = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CursorDisplay();
        if (m_isArea == false)
        {
            return;
        }
        if (Input.GetKeyDown("joystick button 0") || Input.GetMouseButtonDown(0))
        {
            WaitStoreScene();
            m_isArea = false;
            m_isWait = true;
            Debug.Log("ショップへ");
        }
    }

    //ディスプレイにカーソルを表示する
    private void CursorDisplay()
    {
        if(m_isWait == false)
        {
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
