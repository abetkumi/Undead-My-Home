using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightONOFF : MonoBehaviour
{
    GameManager m_gameManager;
    bool m_isLightON = true;
    [SerializeField] GameObject m_spotlight;

    // Start is called before the first frame update
    void Start()
    {
        //�R���|�[�l���g���擾����֐���GetComponent�ł��B
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    void LightON()
    {
        if (m_isLightON == true)
        {
            m_spotlight.SetActive(true);
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_X,
                "���C�g������", true);
        }
        else
        {
            m_spotlight.SetActive(false);
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_X,
                "���C�g������", true);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if ((m_isLightON == true))
            {
                m_isLightON = false;
            }
            else
            {
                m_isLightON = true;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        LightON();
    }
}
