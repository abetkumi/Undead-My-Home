using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PayNorma : MonoBehaviour
{
    GameManager m_gameManager;
    [SerializeField] AudioClip m_paySE;

    bool m_isInArea = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isInArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_isInArea = false;
    }

    void NormaPay()
    {
        float money = m_gameManager.GetMoney();
        float norma = m_gameManager.GetNorma();
        if (money >= norma)
        {
            m_gameManager.SetMoney(-norma);
            int clearCount = m_gameManager.GetClearCount();
            clearCount++;
            m_gameManager.SetClearCount(clearCount);
            GameManager.PlaySE(m_paySE);

            if(clearCount < m_gameManager.GetClearCondition())
            {
                m_gameManager.SetNorma(norma + 50.0f);
            }
        }
    }

    private void Update()
    {
        if (!m_isInArea)
        {
            return;
        }

        if(Input.GetKeyDown("joystick button 0") || Input.GetMouseButtonDown(0))
        {
            NormaPay();
        }
    }
}
