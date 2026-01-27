using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    //アイテム欄のUI
    [SerializeField]
    UI_Item ItemUI;
    Animator m_animator;
    GameManager m_gameManager;
    [SerializeField] AudioClip m_openSE;
    int m_itemIDLength = 4;

    bool m_open = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        ItemUI = GameObject.FindWithTag("ItemUI").GetComponent<UI_Item>();
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
            m_open = false;
            m_animator.SetBool("Open",false);
        }  
    }

    private bool KeyCheck()
    {
        int selectID = m_gameManager.GetSelectItemNo();

        //順番にアイテム欄を確認していって、空いている場所にIDを格納
        for (int i = 0; i < m_itemIDLength; i++)
        {
            if (m_gameManager.GetItemID(selectID) == 11)
            {
                m_animator.SetBool("Open", true);
                GameManager.PlaySE(m_openSE);
                m_gameManager.SetItemID(selectID, -1);
                //UIを更新
                ItemUI.UpdateUI();
                return true;
            }

            selectID++;

            if (selectID > m_itemIDLength - 1)
            {
                //オーバーしたので0に戻す
                selectID = 0;
            }
        }

        //空きがなかった
        return false;
    }

    private void Update()
    {
        if (!m_open)
        {
            return;
        }

        if (Input.GetButtonDown("Action") || Input.GetMouseButtonDown(0))
        {
            KeyCheck();
        }
    }
}
