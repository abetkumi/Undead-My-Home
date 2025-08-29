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

    //���b�Z�[�W������\��
    const float AUTO_OFF_TIME = 3.0f;
    bool m_isAutoOff = false;

    private void Awake()
    {
        //Recttransform���擾���Ă���
        m_searchObjectRectTransform = SearchObject.GetComponent<RectTransform>();
        //�ŏ��͔�\��
        SearchObject.SetActive(false);
    }

    //UI��\�����X�V
    //mode=false...���O�\�����[�h mode=true...�������\�����[�h
    public void SearchUI_On(string text, bool mode)
    {
        if (m_isAutoOff)
        {
            return;
        }

        //�e�L�X�g��\��
        SearchObject.SetActive(true);
        SearchText.text = text;

        //���[�h�ɉ����Ĕw�i�̃T�C�Y��ς���
        if (mode)
        {
            m_searchObjectRectTransform.sizeDelta = MessageSize;
        }
        else
        {
            m_searchObjectRectTransform.sizeDelta = NameSize;
        }

        //Invoke���L�����Z��
        CancelInvoke("SearchUI_Off");
    }

    //UI���\���ɂ���
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
