using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBag : MonoBehaviour
{
    [SerializeField] ItemData m_itemData;
    GameManager m_gameManager;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

        //©g‚ÍƒV[ƒ“‚ğ‚Ü‚½‚¢‚Å‚àíœ‚³‚ê‚È‚¢‚æ‚¤‚É‚·‚é
        DontDestroyOnLoad(gameObject);
    }



    //void SellItemBag()
    //{
    //    float m_sellBag = m_itemData.Items[10].value;
    //    m_gameManager.SetMoney(m_sellBag);
    //    Destroy(gameObject);
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
