using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyItem : MonoBehaviour
{
    [SerializeField] GameObject m_itemObject;
    [SerializeField] GameObject m_itemBuyArea;

    //”„‚è•¨ƒpƒlƒ‹
    [SerializeField] GameObject m_storePanel;
    [SerializeField] GameObject m_storeShoppingPanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    void CreateItem(GameObject item)
    {
        var Item = Instantiate(item);
        Item.transform.position = m_itemBuyArea.transform.position;
    }

    public void OnCancelClick()
    {
        m_storeShoppingPanel.SetActive(false);
        m_storePanel.SetActive(true);
    }

    public void OnClick()
    {
        CreateItem(m_itemObject);
    }
}
