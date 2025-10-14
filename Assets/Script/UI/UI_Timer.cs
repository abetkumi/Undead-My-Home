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
        //ゲーム内開始時に制限時間を設定
        m_currentTime = m_startTimeSecond;
    }

    // Update is called once per frame
    void Update()
    {
        //時間を減らす
        m_currentTime -= Time.deltaTime;

        //0秒未満にならないようにする
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

        //残り時間を秒単位でUIに表示
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
