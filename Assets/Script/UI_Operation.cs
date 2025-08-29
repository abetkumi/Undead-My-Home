using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    //TextMeshProを扱うときに必要

public class UI_Operation : MonoBehaviour
{
    [SerializeField, Header("0=Aボタン 1=Bボタン 2=Xボタン")]
    TextMeshProUGUI[] Texts;

    [SerializeField, Header("押せない時の不透明度")]
    float NoActiveAlpha = 0.4f;

    //ボタンの種類
    public enum Button
    {
        enButton_A,
        enButton_B,
        enButton_X,
    }

    //説明欄の変更
    //mode=false...黒&半透明 true...白&不透明
    public void SetOperation(Button button, string text, bool mode)
    {
        //テキストを更新
        switch (button)
        {
            case Button.enButton_A:
                Texts[(int)button].text = "A:" + text;
                break;
            case Button.enButton_B:
                Texts[(int)button].text = "B:" + text;
                break;
            case Button.enButton_X:
                Texts[(int)button].text = "C:" + text;
                break;
        }

        //色を更新
        if (mode)
        {
            //白&不透明
            Texts[(int)button].color = Color.white;
            Texts[(int)button].alpha = NoActiveAlpha;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        //最初はすべて暗くしておく
        for (int i = 0; i < Texts.Length; i++)
        {
            SetOperation((Button)i, "", false);
        }
    }
}
