using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] GameObject m_UICanvas;
    [SerializeField] GameObject m_storeCanvas;
    [SerializeField] GameObject m_storeOwner;
    [SerializeField] GameObject m_playerObject;

    //���蕨�{�^��
    [SerializeField] GameObject m_storePanel;
    [SerializeField] GameObject m_storeShoppingPanel;
    
    Rigidbody rb;
    LightONOFF m_lightScript;
    GameManager m_gameManager;
    bool m_storeNow = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        m_lightScript = GameObject.FindWithTag("Player").GetComponent<LightONOFF>();
        m_UICanvas = GameObject.FindGameObjectWithTag("UI");
        m_storeCanvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
       // m_lightScript.m_isActionFlag = true;
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_X,
                "�X��Ƙb��", true);
        if (other.CompareTag("Player") && m_storeNow == false)
        {
            if (Input.GetButton("Action"))
            {
                m_storeNow = true;
                m_UICanvas.SetActive(false);
                m_storeCanvas.SetActive(true);
                rb.transform.LookAt(m_storeOwner.transform);
                m_gameManager.SetGameState(GameManager.GameState.enGameState_Shopping);
                Debug.Log("�������J�n");
            }
        }
    }

    //�v���C���[����b�͈͂���o���
    private void OnTriggerExit(Collider other)
    {
        //Actuon�L�[��UI����C�gONOFF�e�L�X�g�ɖ߂�
        if (other.CompareTag("Player"))
        {
            m_lightScript.m_isActionFlag = true;
        }
    }

    //�����������{�^���������ꂽ����
    public void OpenStore()
    {
        m_storeShoppingPanel.SetActive(true);
        m_storePanel.SetActive(false);
    }

    //��������I���{�^���������ꂽ����
    public void CloseStore()
    {
        m_storeNow = false;
        m_UICanvas.SetActive(true);
        m_storeCanvas.SetActive(false);
        m_gameManager.SetGameState(GameManager.GameState.enGameState_Play);
        Cursor.visible = false;  //�}�E�X�J�[�\����\��
        Cursor.lockState = CursorLockMode.Confined; //�}�E�X�J�[�\���̈ړ��𐧌����Ȃ�
    }

    void Shopping()
    {
        Vector3 dir = m_storeOwner.transform.position - rb.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10.0f * Time.fixedDeltaTime));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_storeNow)
        {
            return;
        }

        Cursor.visible = true;  //�}�E�X�J�[�\����\��
        Cursor.lockState = CursorLockMode.None; //�}�E�X�J�[�\���̈ړ��𐧌����Ȃ�

        Shopping();
    }
}
