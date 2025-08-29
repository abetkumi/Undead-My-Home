using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Search : MonoBehaviour
{
    [SerializeField]
    GameObject SearchObject;
    RectTransform m_searchObjectRectTransform;

    [SerializeField]
    TextMeshProUGUI SearchText;

    [SerializeField]
    Vector2 NameSize, MessageSize;

    //メッセージ自動非表示
    const float AUTO_OFF_TIME = 3.0f;
    bool m_isAutoOff = false;

    private void Awake()
    {
        //Recttransformを取得しておく
        m_searchObjectRectTransform = SearchObject.GetComponent<RectTransform>();
        //最初は非表示
        SearchObject.SetActive(false);
    }

    //UIを表示＆更新
    //mode=false...名前表示モード mode=true...説明文表示モード
    public void SearchUI_On(string text, bool mode)
    {
        if (m_isAutoOff)
        {
            return;
        }

        //テキストを表示
        SearchObject.SetActive(true);
        SearchText.text = text;

        //モードに応じて背景のサイズを変える
        if (mode)
        {
            m_searchObjectRectTransform.sizeDelta = MessageSize;
        }
        else
        {
            m_searchObjectRectTransform.sizeDelta = NameSize;
        }

        //Invokeをキャンセル
        CancelInvoke("SearchUI_Off");
    }

    //UIを非表示にする
    public void SearchUI_Off()
    {
        SearchObject.SetActive(false);
        m_isAutoOff = false;
    }

    public void AutoOff()
    {
        Invoke("SearchUI_Off", AUTO_OFF_TIME);
        m_isAutoOff = true;
    }

}
