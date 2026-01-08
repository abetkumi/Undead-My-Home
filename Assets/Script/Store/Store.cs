using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] GameObject m_UICanvas;
    [SerializeField] GameObject m_storeCanvas;
    [SerializeField] GameObject m_storeNPC;
    [SerializeField] GameObject m_storePanel;
    [SerializeField] GameObject m_storeShoppingPanel;
    [SerializeField] GameObject m_spawnPoint;
    [SerializeField] AudioClip m_welcomeSE, m_thankyouSE, m_byeSE;
    [SerializeField] Animator m_shopNPCAnimatior;

    Rigidbody rb;
    GameManager m_gameManager;

    bool m_storeNow = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        m_UICanvas = GameObject.FindGameObjectWithTag("UI");
        m_shopNPCAnimatior = m_storeNPC.GetComponent<Animator>();
        m_storeCanvas.SetActive(false);
        m_gameManager.SetItemDrop(true);
        //ストレージをアイテムとして配置する
        Vector3 itemPos = m_spawnPoint.transform.position;
        itemPos.z -= 3.0f;
        GameObject dropItem = Instantiate(m_gameManager.GetItemData().Items[10].ItemPrefab,
            itemPos, Camera.main.transform.rotation);
    }

    private void OnTriggerStay(Collider other)
    {
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
                "話す", true);
        if (other.CompareTag("Player") && m_storeNow == false)
        {
            if (Input.GetKeyDown("joystick button 0") || Input.GetMouseButtonDown(0))
            {
                m_storeNow = true;
                m_UICanvas.SetActive(false);
                m_storeCanvas.SetActive(true);
                rb.transform.LookAt(m_storeNPC.transform);
                m_gameManager.SetGameState(GameManager.GameState.enGameState_Shopping);
                GameManager.PlaySE(m_welcomeSE);
                m_shopNPCAnimatior.SetBool("Shop", true);

                Debug.Log("買い物開始");
            }
        }
    }

    public void OpenStore()
    {
        m_storeShoppingPanel.SetActive(true);
        m_storePanel.SetActive(false);
    }

    public void CloseStore()
    {
        m_storeNow = false;
        m_UICanvas.SetActive(true);
        m_storeCanvas.SetActive(false);
        m_gameManager.SetGameState(GameManager.GameState.enGameState_Play);
        m_shopNPCAnimatior.SetBool("Shop", false);
        Cursor.visible = false;  
        Cursor.lockState = CursorLockMode.Confined;
        GameManager.PlaySE(m_byeSE);
    }

    void Shopping()
    {
        Vector3 dir = m_storeNPC.transform.position - rb.position;
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

        Cursor.visible = true;  
        Cursor.lockState = CursorLockMode.None; 

        Shopping();
    }
}
