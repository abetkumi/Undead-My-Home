using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyItem : MonoBehaviour
{
    [SerializeField] GameObject m_itemObject;
    [SerializeField] GameObject m_itemBuyArea;

    //”„‚è•¨ƒpƒlƒ‹
    [SerializeField] GameObject m_storePanel;
    [SerializeField] GameObject m_storeShoppingPanel;
    [SerializeField] float m_price;

    [SerializeField] Button m_focusButton_ShoppingOpen;
    [SerializeField] AudioClip m_thankyouSE;

    void CreateItem(GameObject item)
    {
        var Item = Instantiate(item);
        Item.transform.position = m_itemBuyArea.transform.position;
    }

    async public void OnCancelClick()
    {
        m_storeShoppingPanel.SetActive(false);
        m_storePanel.SetActive(true);

        await UniTask.Delay(100);
        m_focusButton_ShoppingOpen = m_focusButton_ShoppingOpen.GetComponent<Button>();
        m_focusButton_ShoppingOpen.Select();
    }

    public void OnClick()
    {
        GameManager gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        float money = gameManager.GetMoney();
        if (money >= m_price)
        {
            GameManager.PlaySE(m_thankyouSE);
            gameManager.SetMoney(-m_price);
            CreateItem(m_itemObject);
        }
    }
}
