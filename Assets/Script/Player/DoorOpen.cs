using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    Animator m_animator;
    GameManager m_gameManager;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        m_animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
                "ドアを開ける", true);
        if (other.CompareTag("Player"))
        {
            if (Input.GetKey("joystick button 0") || Input.GetMouseButton(0))
            {
                m_animator.SetBool("Open",true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_animator.SetBool("Open",false);
        }  
    }
}
