using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class UI_Pause : MonoBehaviour
{
    [SerializeField] GameObject m_playerParentObject;
    [SerializeField] GameObject m_pauseObject;
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_gameManagerObject;
    [SerializeField] public Button m_focusButton_Title;
    [SerializeField] Button m_focusButton_GameBack;

    // Start is called before the first frame update
    void Start()
    {
        m_focusButton_Title = m_focusButton_Title.GetComponent<Button>();
        m_focusButton_GameBack = m_focusButton_GameBack.GetComponent<Button>();
        m_pauseObject.SetActive(false);
    }

    //Yesボタンが押されたのでタイトルに戻る
    public async void TitleBackButton()
    {
        //ボタンのselectedを初期化
        EventSystem.current.SetSelectedGameObject(null);
        //タイトルボタンをセレクトアニメーションさせる
        m_focusButton_Title.Select();
        Time.timeScale = 1.0f;
        m_pauseObject.SetActive(false);
        //プレイヤーとゲームマネージャーをシーン切り替えで消えるように切り替える
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(m_playerParentObject, activeScene);


        await UniTask.Delay(500);
        //メインゲームシーンに移動する
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // 生成したオブジェクトのFadeStart関数を呼び出す
        fadeObject.GetComponent<FadeScene>().FadeStart("TitleScene", Color.black, true);
        Debug.Log("タイトルに戻る");

    }

    //Noボタンが押されたのでゲームに戻る
    public void GameBackButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameManager m_gameManager = m_gameManagerObject.GetComponent<GameManager>();
        m_gameManager.SetGameState(GameState.enGameState_Play);
        m_pauseObject.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
