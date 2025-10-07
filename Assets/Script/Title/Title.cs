using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] GameObject m_fadeCanvas;    //フェード演出用オブジェクト
    [SerializeField] Button m_focusButton_Start;
    [SerializeField] Button m_focusButton_End;

    bool m_sceneChange = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnClickStartButton()
    {
        //シーン切り替え中は何もしない
        if (m_sceneChange)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        m_focusButton_Start.Select();
        //フェード演出用オブジェクトを生成
        GameObject fade = Instantiate(m_fadeCanvas);
        //生成したオブジェクトのFadeStart関数を呼び出す
        fade.GetComponent<FadeScene>().FadeStart(Color.black, false);

        m_sceneChange = true;
      
        
    }

    public void OnClickEndButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
