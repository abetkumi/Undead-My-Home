using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Item : MonoBehaviour
{
    GameManager m_gameManager;

    [SerializeField]
    GameObject[] ItemCamara; //UI用カメラ

    [SerializeField]
    TextMeshProUGUI ItemNameText, ItemMessageText;

    [SerializeField]
    Vector3 ItemOffset; //アイテム生成座標の補正量（共通）

    //UI用に生成したアイテム
    GameObject[] m_itemObject;

    //表示内容を更新する　アイテムインベントリを更新したタイミングで呼ぶ
    public void UpdateUI()
    {
        //初期化
        if (m_gameManager == null)
        {
            m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            //要素数はカメラの数と同じ
            m_itemObject = new GameObject[ItemCamara.Length];
        }

        //変数を用意する
        Vector3 itemPos;
        int itemNo;
        int selectNo = m_gameManager.GetSelectItemNo();

        //アイテム欄を更新する
        for (int i = 0; i < ItemCamara.Length; i++)
        {
            //指定スロットのアイテム番号を取得
            itemNo = m_gameManager.GetItemID(selectNo);
            //アイテムをいったん削除
            Destroy(m_itemObject[i]);

            //名前や説明欄の更新
            if (selectNo == m_gameManager.GetSelectItemNo())
            {
                //表示の有無
                if (itemNo == -1)
                {
                    //アイテムがないため説明欄は非表示
                    ItemNameText.enabled = false;
                    ItemMessageText.enabled = false;

                    //操作説明の更新
                    m_gameManager.GetOperationUI().SetOperation
                        (UI_Operation.Button.enButton_B, "", false);

                }
                else
                {
                    //説明欄を表示
                    ItemNameText.enabled = true;
                    ItemMessageText.enabled = true;

                    //テキストを更新
                    ItemNameText.text = m_gameManager.GetItemData().Items[itemNo].ItemName;
                    ItemMessageText.text = m_gameManager.GetItemData().Items[itemNo].ItemExplanation;

                    //操作説明の更新
                    m_gameManager.GetOperationUI().SetOperation
                        (UI_Operation.Button.enButton_B, "捨てる", true);
                }
            }

            //セレクト番号を更新
            selectNo++;
            if (selectNo > ItemCamara.Length - 1)
            {
                selectNo = 0;
            }
            if (itemNo == -1)
            {
                //アイテムがないためここで終了
                continue;
            }

            //アイテムを生成
            //まずは座標を決める
            itemPos = ItemCamara[i].transform.position;
            itemPos += ItemOffset;
            itemPos += m_gameManager.GetItemData().Items[itemNo].UI_Offset;

            //生成
            GameObject item = Instantiate(m_gameManager.GetItemData().Items[itemNo].ItemPrefab,
                itemPos, Quaternion.identity);

            //アイテムを覚えておく
            m_itemObject[i] = item;

            //回転
            item.transform.Rotate(m_gameManager.GetItemData().Items[itemNo].UI_Rotation,
                Space.World);
            //大きさ調整
            item.transform.localScale = m_gameManager.GetItemData().Items[itemNo].UI_Scale;

            //レイヤーをUI用に変更（名前で検索)
            item.layer = LayerMask.NameToLayer("UI_Item");
        }
    }
}
