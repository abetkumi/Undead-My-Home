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
    private float m_currentTimeSecond;
    public TextMeshProUGUI m_timerText;

    // Start is called before the first frame update
    void Start()
    {
        //ゲーム内開始時に制限時間を設定
        m_currentTime = m_startTime;
        m_currentTimeSecond = m_startTimeSecond;
    }

    public void ResetTimer()
    {
        m_currentTime = m_startTime;
        m_currentTimeSecond = m_startTimeSecond;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.GetGameState() != GameManager.GameState.enGameState_Play)
        {
            return;
        }

        //時間を減らす
        m_currentTimeSecond -= Time.deltaTime;

        //0秒未満にならないようにする
        if (m_currentTimeSecond < -1) 
        {
            m_currentTimeSecond = 0;

            if(m_currentTime <= 0)
            {
                GameOver gameOver =
                    GameObject.FindGameObjectWithTag("GameOver").GetComponent<GameOver>();
                gameOver.SetGameOver();
                Debug.Log("Time up!");
            }
            else
            {
                m_currentTime--;
                m_currentTimeSecond = m_startTimeSecond;
            }
        }

        //残り時間を秒単位でUIに表示
        if (m_currentTimeSecond < 9)
        {
            m_timerText.text = m_currentTime + ":0" + Mathf.Ceil(m_currentTimeSecond).ToString();
        }
        else
        {
            m_timerText.text = m_currentTime + ":" + Mathf.Ceil(m_currentTimeSecond).ToString();
        }
    }
}
