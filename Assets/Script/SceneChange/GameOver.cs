using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
    [SerializeField] GameObject m_playerObject;
    [SerializeField] GameObject m_timerObject;
    [SerializeField] GameObject m_cameraObject;
    CameraCulling m_cameraCulling;
    [SerializeField] GameObject m_weaponObject;
    PlayerAttack m_playerAttack;
    bool m_isGameOver = false;

    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetGameOver();
            Debug.Log("Dead");
        }
    }

    //ゲームオーバー処理
    public async void SetGameOver()
    {
        //ゲームマネージャーを取得
        GameManager m_gameManager = 
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        m_gameManager.SetGameState(GameManager.GameState.enGameState_GameOver);

        m_cameraCulling = m_cameraObject.GetComponent<CameraCulling>();
        m_cameraCulling.ShowPlayerBody();

        m_playerAttack = m_weaponObject.GetComponent<PlayerAttack>();
        m_playerAttack.NormalScale();

        Destroy(m_timerObject);

        Vector3 m_camaraPos = m_playerObject.transform.position;
        m_camaraPos.y += 4.0f;
        m_camaraPos += m_playerObject.transform.forward * 3.0f;
        m_cameraObject.transform.position = m_camaraPos;
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

        await UniTask.Delay(1050);
        GameObject m_gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        Destroy(m_playerObject);
        Destroy(m_gameManagerObject);
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
