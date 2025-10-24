using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_timerObject;
    bool m_isGameClaer = false;
    int m_normaCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetButtonDown("Action"))
        {
            if(m_normaCount == 3)
            {
                SetGameClear();
                Debug.Log("Clear");
            }
            else
            {
                
                Debug.Log("ショップへ");
            }
        }
    }

    async public void SetGameClear()
    {
        //ゲームマネージャーを取得
        GameManager m_gameManager =
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        m_gameManager.SetGameState(GameManager.GameState.enGameState_GameClear);

        Destroy(m_timerObject);

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
