using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] float m_startTime = 4.0f;
    [SerializeField] float m_startTimeSecond = 59.0f;
    private float m_currentTime;
    public TextMeshProUGUI m_timerText;

    // Start is called before the first frame update
    void Start()
    {
        //�Q�[�����J�n���ɐ������Ԃ�ݒ�
        m_currentTime = m_startTimeSecond;
    }

    // Update is called once per frame
    void Update()
    {
        //���Ԃ����炷
        m_currentTime -= Time.deltaTime;

        //0�b�����ɂȂ�Ȃ��悤�ɂ���
        if (m_currentTime < -1) 
        {
            m_currentTime = 0;

            if(m_startTime <= 0)
            {
                GameOver gameOver =
                    GameObject.FindGameObjectWithTag("GameOver").GetComponent<GameOver>();
                gameOver.SetGameOver();
                Debug.Log("Time up!");
            }
            else
            {
                m_startTime--;
                m_currentTime = m_startTimeSecond;
            }
        }

        //�c�莞�Ԃ�b�P�ʂ�UI�ɕ\��
        if (m_currentTime < 9)
        {
            m_timerText.text = m_startTime + ":0" + Mathf.Ceil(m_currentTime).ToString();
        }
        else
        {
            m_timerText.text = m_startTime + ":" + Mathf.Ceil(m_currentTime).ToString();
        }
    }
}
