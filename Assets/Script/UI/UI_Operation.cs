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

    private bool m_usingGamepad = true; // 現在どちらの入力を使っているか

    //ボタンの種類
    public enum Button
    {
        enButton_B,
        enButton_Y,
        enButton_L2,
        enButton_R3,
        enButton_L3,
        enMoney,
    }

    Dictionary<Button, (string gamepad, string keyboard)> buttonLabels
    = new Dictionary<Button, (string, string)>
{
    { Button.enButton_B,  ("B",  "左クリック") },
    { Button.enButton_Y,  ("Y",  "G") },
    { Button.enButton_L2, ("L2", "E") },
    { Button.enButton_R3, ("R3", "Q") },
    { Button.enButton_L3, ("L3", "F") },
};


    class OperationInfo
    {
        public Button button;
        public string text;
        public bool mode;
    }


    List<OperationInfo> operations = new List<OperationInfo>();


    //説明欄の変更
    //mode=false...黒&半透明 true...白&不透明
    public void SetOperation(Button button, string text, bool mode)
    {
        //テキストを更新
        var info = operations.Find(o => o.button == button);
        if (info == null)
        {
            info = new OperationInfo { button = button };
            operations.Add(info);
        }

        info.text = text;
        info.mode = mode;

        UpdateOperationUI(info);
    }

    void UpdateOperationUI(OperationInfo info)
    {
        if (buttonLabels.TryGetValue(info.button, out var label))
        {
            string prefix = m_usingGamepad ? label.gamepad : label.keyboard;
            Texts[(int)info.button].text = $"{prefix}:{info.text}";
        }
        else
        {
            Texts[(int)info.button].text = info.text;
        }

        var color = Texts[(int)info.button].color;
        color.a = info.mode ? 1.0f : NoActiveAlpha;
        Texts[(int)info.button].color = color;
    }
    public void OnInputDeviceChanged(bool usingGamepad)
    {
        m_usingGamepad = usingGamepad;

        foreach (var info in operations)
        {
            UpdateOperationUI(info);
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
        return Input.GetAxis("Horizontal") != 0 ||
               Input.GetAxis("Vertical") != 0 ||
               Input.GetMouseButtonDown(0) ||
               Input.GetMouseButtonDown(1);
    }

    private bool CheckGamepadInput()
    {
        // 代表的なゲームパッドの入力を検出
        return Input.GetAxis("Horizontal_Pad") != 0 ||
               Input.GetAxis("Vertical_Pad") != 0;
    }

    private void LateUpdate()
    {
        //キーボード・マウスの入力検知
        if (CheckKeyboardMouseInput())
        {
            OnInputDeviceChanged(false);
        }

        //ゲームパッドの入力検知
        if (CheckGamepadInput())
        {
            OnInputDeviceChanged(true);
        }
    }
}
