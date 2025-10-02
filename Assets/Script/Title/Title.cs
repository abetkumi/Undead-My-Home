using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] Button m_focusButton_Start;
    [SerializeField] Button m_focusButton_End;
    bool m_startLoading = false;
    // Start is called before the first frame update
    void Start()
    {
        m_startLoading = false;
    }

    async public void OnClickStartButton()
    {
        if (!m_startLoading)
        {
            m_startLoading = true;
            EventSystem.current.SetSelectedGameObject(null);
            m_focusButton_Start.Select();
            await UniTask.Delay(3000);
            //メインゲームシーンに移動する
            await SceneManager.LoadSceneAsync("SampleScene").ToUniTask();
            Debug.Log("ゲームスタート!");  // ログを出力
        }
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
