using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayNorma : MonoBehaviour
{
    GameManager m_gameManager;
    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float money = m_gameManager.GetMoney();
            float norma = m_gameManager.Get
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
