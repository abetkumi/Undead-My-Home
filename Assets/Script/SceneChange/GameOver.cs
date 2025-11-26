using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    CameraCulling m_cameraCulling;
    PlayerAttack m_playerAttack;
    bool m_isGameOver = false;

    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.Dead();
            Debug.Log("Dead");
        }
        else if(other.CompareTag("Item"))
        {
            Destroy(other.gameObject);
        }
    }

    //ゲームオーバー処理
    public async void SetGameOver()
    {
        //ゲームマネージャーを取得
        GameManager m_gameManager = 
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        m_gameManager.SetGameState(GameManager.GameState.enGameState_GameOver);

        //プレイヤーをカメラに映るようにする
        m_cameraCulling = Camera.main.GetComponent<CameraCulling>();
        m_cameraCulling.ShowPlayerBody();

        //武器の縮尺をもとに戻す

        GameObject m_attackObject = GameObject.FindWithTag("Weapon");
        if(m_attackObject != null)
        {
            m_playerAttack = m_attackObject.GetComponent<PlayerAttack>();
            m_playerAttack.NormalScale();
        }

        GameObject m_timerObject = GameObject.FindWithTag("Timer");
        Destroy(m_timerObject);

        GameObject m_playerObject = GameObject.FindWithTag("Player");
        Vector3 m_camaraPos = m_playerObject.transform.position;
        m_camaraPos.y += 4.0f;
        m_camaraPos += m_playerObject.transform.forward * 3.0f;
        Camera.main.transform.position = m_camaraPos;
        Camera.main.GetComponent<GameCamera>().FocusStart(m_playerObject.transform.position, 3.0f, 5.0f);
        m_playerObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        await UniTask.Delay(1000);
        // シーン切替
        // フェード演出用オブジェクトを生成
        GameObject fadeObject = Instantiate(m_fadeCanvas);
        // 生成したオブジェクトのFadeStart関数を呼び出す
        fadeObject.GetComponent<FadeScene>().FadeStart("GameOverScene", Color.black, true);

        m_isGameOver = true;
        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(gameObject);        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isGameOver == false)
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
