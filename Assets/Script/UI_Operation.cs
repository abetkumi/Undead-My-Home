using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    //TextMeshPro�������Ƃ��ɕK�v

public class UI_Operation : MonoBehaviour
{
    [SerializeField, Header("0=A�{�^�� 1=B�{�^�� 2=X�{�^��")]
    TextMeshProUGUI[] Texts;

    [SerializeField, Header("�����Ȃ����̕s�����x")]
    float NoActiveAlpha = 0.4f;

    //�{�^���̎��
    public enum Button
    {
        enButton_A,
        enButton_B,
        enButton_X,
    }

    //�������̕ύX
    //mode=false...��&������ true...��&�s����
    public void SetOperation(Button button, string text, bool mode)
    {
        //�e�L�X�g���X�V
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

        //�F���X�V
        if (mode)
        {
            //��&�s����
            Texts[(int)button].color = Color.white;
            Texts[(int)button].alpha = NoActiveAlpha;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        //�ŏ��͂��ׂĈÂ����Ă���
        for (int i = 0; i < Texts.Length; i++)
        {
            SetOperation((Button)i, "", false);
        }
    }
}
