using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStorage : MonoBehaviour
{
    [SerializeField] int[]m_itemStorageID;
    [SerializeField] UI_Item m_itemUI;
    GameManager m_gameManager;
    Player m_player;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager=GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        m_player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        //アイテムがあるか確認
        if (m_gameManager.GetItemID(m_gameManager.GetSelectItemNo()) == -1)
        {
            Debug.Log("【エラー】" + m_gameManager.GetSelectItemNo() + "番にアイテムがありません！");
            return;
        }

        m_gameManager.SetItemDrop(false);

        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_Y,
            "アイテムを収納する", true);

        if ((Input.GetKey("joystick button 1") || Input.GetKey(KeyCode.G)))
        {
            int selectID = 0;

            //順番にアイテム欄を確認していって、空いている場所にIDを格納
            for (int i = 0; i < m_itemStorageID.Length; i++)
            {
                int itemID = m_gameManager.GetItemID(m_gameManager.GetSelectItemNo());
                if (itemID >= 7 && itemID < 10)
                {
                    Debug.Log("使用アイテムです");
                    break;
                }

                if (m_itemStorageID[selectID] == -1)
                {

                    //空きがあるのでアイテムIDを格納
                    m_itemStorageID[selectID] = itemID;

                    ////効果音再生
                    //GameManager.PlaySE(ItemGetSE);
                    //アイテムを捨てる場合、そのアイテムの重量分減算する。
                    m_player.ItemWeightAdd(m_gameManager.GetItemData().Items[m_gameManager.GetItemID(m_gameManager.GetSelectItemNo())].weight, false);

                    //UIを更新
                    //アイテム欄のIDをリセット
                    m_gameManager.SetItemID(m_gameManager.GetSelectItemNo(), -1);
                    m_itemUI.UpdateUI();
                }

                selectID++;

                if (selectID > m_itemStorageID.Length - 1)
                {
                    //オーバーしたので0に戻す
                    selectID = 0;
                }
            }
            //UIを更新
            m_itemUI.UpdateUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_gameManager.SetItemDrop(true);
        //UIを更新
        m_itemUI.UpdateUI();
    }
}
