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

        //自身はシーンをまたいでも削除されないようにする
        DontDestroyOnLoad(gameObject);
    }

    async void Fade()
    {
        //フェードが開始していないなた中断
        if (m_fadeStart == false)
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
                await SceneManager.LoadSceneAsync(m_sceneName).ToUniTask();
                Debug.Log("ゲームスタート!");  // ログを出力
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

        //最後に不透明度をせてチスル
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
        Fade();
    }
}
