using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class StoreDoor : MonoBehaviour
{
    //アイテム欄のUI
    [SerializeField]
    UI_Item ItemUI;
    Animator m_animator;
    GameManager m_gameManager;
    [SerializeField] AudioClip m_openSE, m_closeSE;

    bool m_open = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        m_animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_B,
                "ドアを開ける", true);
        if (other.CompareTag("Player"))
        {
            m_open = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_B,
                "", true);
            m_open = false;
            m_animator.SetBool("Open", false);
        }
    }

    void CloseDoorSE()
    {
        GameManager.PlaySE(m_closeSE);
    }

    private void Update()
    {
        if (!m_open)
        {
            return;
        }

        if (Input.GetButtonDown("Action") || Input.GetMouseButtonDown(0))
        {
            m_animator.SetBool("Open", true);
            GameManager.PlaySE(m_openSE);
        }
    }
}