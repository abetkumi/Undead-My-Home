using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneBack : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;
   

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetButtonDown("Action"))
        {
            SetMainGameBack();
            Debug.Log("SampleSceneに戻る");
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
}
