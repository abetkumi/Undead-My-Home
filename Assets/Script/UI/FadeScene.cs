using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScene : MonoBehaviour
{
    bool m_fadeStart = false;   //trueなら一連の処理を開始
    bool m_fadeMode = false;    //falseなら暗くなるtrueなら明るくなる
    float m_alpha = 0.0f;       //画像の不透明度

    [SerializeField]
    float FadeSpeed = 1.0f; //フェードの速度

    // 遷移先のシーン名を保存
    string m_sceneName;
    //自身が使用するImageを保存
    Image m_image;
    //falseならマテリアルを使用trueならImageを使用
    bool m_mode = false;

    bool m_sceneChange = false;

    GameObject m_playerObject;
    Vector3 m_warppositon;

    //フェード開始
    public void FadeStart(string sceneName, Color color, bool mode)
    {
        //フェード開始の準備をする
        m_fadeStart = true;
        m_sceneName = sceneName;
        m_mode = mode;

        //自分の子オブジェクトにアタッチされているImageを取得する
        m_image = transform.GetChild(0).GetComponent<Image>();
        if (m_mode)
        {
            //通常フェード
            m_image.material = null;
            m_image.color = color;
        }
        else
        {
            //マテリアルを初期化
            m_image.material.SetFloat("_Border", 0.0f);
            m_image.material.SetColor("_Color", color);
            //自身のRenderCameraにメインカメラを設定する
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
        m_sceneChange = true;
        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(gameObject);
    }
    public void FadeStart(Vector3 position,Color color, bool mode)
    {
        //フェード開始の準備をする
        m_fadeStart = true;
        m_mode = mode;
        m_warppositon = position;

        //自分の子オブジェクトにアタッチされているImageを取得する
        m_image = transform.GetChild(0).GetComponent<Image>();
        if (m_mode)
        {
            //通常フェード
            m_image.material = null;
            m_image.color = color;
        }
        else
        {
            //マテリアルを初期化
            m_image.material.SetFloat("_Border", 0.0f);
            m_image.material.SetColor("_Color", color);
            //自身のRenderCameraにメインカメラを設定する
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        m_sceneChange = false;
        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(gameObject);
    }
    void Fade()
    {

        //フェードが開始していないため中断
        if (!m_fadeStart)
        {
            return;
        }

        //自身のRenderCameraにメインカメラを設定する
        if (GetComponent<Canvas>().worldCamera == null &&
            m_mode == false)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        //フェード処理
        if (m_fadeMode == false)
        {
            //画面を暗くする
            m_alpha += FadeSpeed * Time.deltaTime;
            //完全に暗くなったのでシーンを変更する
            if (m_alpha >= 1.0f)
            {
                //メインゲームシーンに移動する
                SceneManager.LoadSceneAsync(m_sceneName);
                Debug.Log("ゲームスタート!");  // ログを出力
                //明るくするモードに変更
                m_fadeMode = true;
                
                if(GameManager.GetGameState() == GameManager.GameState.enGameState_GameClear || 
                    GameManager.GetGameState() == GameManager.GameState.enGameState_GameOver)
                {
                    GameObject m_gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
                    GameObject m_playerObject = GameObject.FindGameObjectWithTag("Player");
                    Destroy(m_playerObject);
                    Destroy(m_gameManagerObject);
                }
            }
        }
        else
        {
            //画面を明るくする
            m_alpha -= FadeSpeed * Time.deltaTime;
            //完全に明るくなったので自身を削除する
            if (m_alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
        }

        //最後に不透明度を設定する
        if (m_mode)
        {
            Color nowColor = m_image.color;
            nowColor.a = m_alpha;
            m_image.color = nowColor;
        }
        else
        {
            m_image.material.SetFloat("_Border", m_alpha);
        }
    }

    void FadeNoScene(Vector3 position)
    {
        //フェードが開始していないため中断
        if (m_fadeStart)
        {
            return;
        }

        m_playerObject = GameObject.FindWithTag("Player");
        //自身のRenderCameraにメインカメラを設定する
        if (GetComponent<Canvas>().worldCamera == null &&
            m_mode == false)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        //フェード処理
        if (m_fadeMode == false)
        {
            //画面を暗くする
            m_alpha += FadeSpeed * Time.deltaTime;
            //完全に暗くなったのでシーンを変更する
            if (m_alpha >= 1.0f)
            {
                m_playerObject.transform.position = position;
                //明るくするモードに変更
                m_fadeMode = true;
            }
        }
        else
        {
            //画面を明るくする
            m_alpha -= FadeSpeed * Time.deltaTime;
            //完全に明るくなったので自身を削除する
            if (m_alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
        }

        //最後に不透明度を設定する
        if (m_mode)
        {
            Color nowColor = m_image.color;
            nowColor.a = m_alpha;
            m_image.color = nowColor;
        }
        else
        {
            m_image.material.SetFloat("_Border", m_alpha);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_sceneChange)
        {
            Fade();
        }
        else
        {
            FadeNoScene(m_warppositon);
        }
        
    }
}
