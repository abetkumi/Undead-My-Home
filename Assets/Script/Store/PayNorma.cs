using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PayNorma : MonoBehaviour
{
    GameManager m_gameManager;
    [SerializeField] GameObject m_mainBackObject;
    MainSceneBack m_mainBack;
    [SerializeField] AudioClip m_paySE;

    bool m_isInArea = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        m_mainBack = m_mainBackObject.GetComponent<MainSceneBack>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(m_gameManager.GetClearCount() != m_mainBack.GetClearCount())
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_B,
                "ƒmƒ‹ƒ}‚ðŽx•¥‚¤", true);
            m_isInArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_B,
                    "", true);
            m_isInArea = false;
        }
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
                m_gameManager.SetNorma(norma + 100.0f);
            }
        }
    }

    private void Update()
    {
        if (!m_isInArea)
        {
            return;
        }

        if(Input.GetButtonDown("Action") || Input.GetMouseButtonDown(0))
        {
            NormaPay();
        }
    }
}
