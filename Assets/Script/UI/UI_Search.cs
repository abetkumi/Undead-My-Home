using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Search : MonoBehaviour
{
    public Transform m_targetTran;

    //テキストを変更する
    public void ShowText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = '$' + text;
    }

    private void Update()
    {
        //オブジェクトの少し上にテキストが写るように場所を取得
        transform.position = RectTransformUtility.WorldToScreenPoint(
            Camera.main,
            m_targetTran.position + Vector3.up);
    }

}
