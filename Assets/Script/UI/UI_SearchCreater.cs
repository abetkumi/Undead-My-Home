using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class UI_SearchCreater : MonoBehaviour
{
    //Canvasの場所を取得
    public RectTransform m_canvasRect;
    //UIテキスト生成用
    [SerializeField] UI_Search m_UI_SearchPrefab;
   
    UI_Search m_UI_Search;
    ItemObject itemObject;
    float m_itemID;

    private void Start()
    {
        m_canvasRect = GameObject.FindWithTag("UI").GetComponent<RectTransform>();
        //テキストをCanvasに生成する
        m_UI_Search = Instantiate(m_UI_SearchPrefab, m_canvasRect);
        //オブジェクトの少し上に見えるように移動
        m_UI_Search.m_targetTran = transform;

        itemObject = gameObject.GetComponent<ItemObject>();
        m_itemID = itemObject.GetItemValue();


        //サーチボタンが押されるまで非表示
        m_UI_Search.gameObject.SetActive(false);
    }

    //収集アイテムの情報表示
    async public void ShowValue()
    {
        if(m_UI_Search == null)
        {
            return;
        }

        //テキストを表示
        m_UI_Search.gameObject.SetActive(true);
        //価格を表示
        m_UI_Search.ShowText(m_itemID.ToString());

        //2秒待機
        await UniTask.Delay(2000);

        //アイテムがまだある場合
        if (m_UI_Search == null)
        {
            return;
        }
        //情報を非表示
        m_UI_Search.gameObject.SetActive(false);

    }

    //アイテムが非表示の場合
    private void OnDisable()
    {
        if(m_UI_Search == null)
        {
            return;
        }

        //テキストも非表示
        m_UI_Search.gameObject.SetActive(false);
    }

    //アイテムが破棄された場合
    private void OnDestroy()
    {
        if(m_UI_Search == null)
        {
            return;
        }

        //テキストも破棄する
        Destroy(m_UI_Search.gameObject);
    }
}
