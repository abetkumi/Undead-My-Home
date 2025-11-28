using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    Animator m_animator;
    LightONOFF m_lightScript;
    GameManager m_gameManager;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        m_animator = GetComponent<Animator>();
        m_lightScript = GameObject.FindWithTag("Player").GetComponent<LightONOFF>();
    }

    private void OnTriggerStay(Collider other)
    {
        m_lightScript.m_isActionFlag = false;
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_X,
                "ƒhƒA‚ðŠJ‚¯‚é", true);
        if (other.CompareTag("Player"))
        {
            if (Input.GetButton("Action"))
            {
                m_animator.SetBool("Open",true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_lightScript.m_isActionFlag = true;
            m_animator.SetBool("Open",false);
        }  
    }
}
