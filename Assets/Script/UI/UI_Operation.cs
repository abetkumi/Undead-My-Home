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

    private bool m_usingGamepad = false; // 現在どちらの入力を使っているか
    private float lastInputTime = 0f;

    //ボタンの種類
    public enum Button
    {
        enButton_A,
        enButton_B,
        enButton_X,
        enButton_Y,
        enButton_RB,
    }

    //説明欄の変更
    //mode=false...黒&半透明 true...白&不透明
    public void SetOperation(Button button, string text, bool mode)
    {
        //テキストを更新
        switch (button)
        {
            case Button.enButton_A:
                if (m_usingGamepad == true)
                {
                    Texts[(int)button].text = "A:" + text;
                }
                else
                {
                    Texts[(int)button].text = "左クリック:" + text;
                }
                break;
            case Button.enButton_B:
                if (m_usingGamepad == true)
                {
                    Texts[(int)button].text = "B:" + text;
                }
                else
                {
                    Texts[(int)button].text = "Q:" + text;
                }
                break;
            case Button.enButton_X:
                if (m_usingGamepad == true)
                {
                    Texts[(int)button].text = "X:" + text;
                }
                else
                {
                    Texts[(int)button].text = "F:" + text;
                }
                break;
            case Button.enButton_Y:
                if (m_usingGamepad == true)
                {
                    Texts[(int)button].text = "Y:" + text;
                }
                else
                {
                    Texts[(int)button].text = "G:" + text;
                }
                break;
            case Button.enButton_RB:
                if (m_usingGamepad == true)
                {
                    Texts[(int)button].text = "RB:" + text;
                }
                else
                {
                    Texts[(int)button].text = "E:" + text;
                }
                break;
        }

        //色を更新
        if (mode)
        {
            //白&不透明
            Texts[(int)button].color = Color.white;
            Texts[(int)button].alpha = 1.0f;
        }
        else
        {
            //黒&半透明
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

    private bool CheckKeyboardMouseInput()
    {
        // キーボードまたはマウスが押されたら true
        return Input.anyKeyDown;
    }

    private bool CheckGamepadInput()
    {
        // 代表的なゲームパッドの入力を検出
        return Input.GetButtonDown("Action") ||
               Input.GetButtonDown("Jump");
    }

    private void Update()
    {
        //ゲームパッドの入力検知
        if (CheckGamepadInput())
        {
            if (!m_usingGamepad)
            {
                m_usingGamepad = true;
            }
            lastInputTime = Time.time;
        }

        //キーボード・マウスの入力検知
        if (CheckKeyboardMouseInput())
        {
            if (m_usingGamepad)
            {
                m_usingGamepad = false;
            }
            lastInputTime = Time.time;
        }
    }
}
