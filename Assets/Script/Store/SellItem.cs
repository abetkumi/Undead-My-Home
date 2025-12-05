using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItem : MonoBehaviour
{
    GameManager m_gameManager;
    Player player;
    [SerializeField] AudioClip m_moneyGetSE;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemSell(other);
        }
    }

    void ItemSell(Collider other)
    {
        int itemID = other.GetComponent<ItemObject>().GetItemID();
        float money = m_gameManager.GetItemData().Items[itemID].value;
        m_gameManager.SetMoney(money);

        GameManager.PlaySE(m_moneyGetSE);
        Destroy(other.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
