using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour
{
    [SerializeField, Tooltip("-1は調べられるアイテム0以上は取得できるアイテム")]
    int ItemID = -1;    //アイテム識別用番号
    [SerializeField] ItemData itemDataBase; //アイテムリスト

    [SerializeField] string Name; //カーソルを合わせたときに表示する名前
    [SerializeField, Multiline(3)] protected string Explanation; //調べたときに表示する名前

    Outline m_outline;
    const float SELECT_OUTLINE_WIDTH = 16.0f;

    const float ITEMDROP_POWER = 20.0f;     //アイテムを捨てる時にかける力
    const float ITEMDROP_TORQUE = 60.0f;    //アイテムを捨てる時にかける回転量

    [SerializeField] bool IsCheck = false;

    //ゲームマネージャー
    protected GameManager m_gameManager;

    public void SetIsCheck(bool flag)
    {
        IsCheck = flag;
    }



    //GameObject m_platformObj = null;  //自分が置かれている

    private void Awake()
    {
        //アウトラインの初期化
        m_outline = GetComponent<Outline>(); 
        m_outline.OutlineMode = Outline.Mode.OutlineVisible;
        m_outline.OutlineColor = Color.red;
        m_outline.OutlineWidth = SELECT_OUTLINE_WIDTH;
        m_outline.enabled = false;
        //ゲームマネージャーを取得
        //【ヒント1】GameManagerのタグ名は「GameController」です
        //【ヒント2】コンポーネントを取得する関数はGetComponentです。
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    //アイテムを調べた時の仮想関数
    public virtual void ItemCheck()
    {
        //調べた時の基本処理
        ItemGet();
    }

    //アイテムを調べて時の基本処理
    public void ItemGet()
    {
        //調べられない状態
        if (IsCheck)
        {
            return;
        }

        if (ItemID == -1)
        {
            //アイテムを調べた

            //デバッグ用アイテム名と説明文をコンソールに出力
            //Debug/Log("アイテム名:"+Name+"\n説明文:"+Explanation);

            //獲得できないアイテムなので説明を表示
            m_gameManager.GetSearchUI().SearchUI_On(Explanation, true);
            m_gameManager.GetSearchUI().AutoOff();

            //効果音を再生
            //GameManager.PlaySE(m_gameManager.GetEnterSE());
        }
        else
        {
            //アイテムを取得
            //【ヒント】ゲームマネージャーのGetItem関数を使おう
            bool isGet = m_gameManager.GetItem(ItemID);

            //アイテム欄に空きがあったかどうかで分岐
            if (isGet)
            {
                //アイテムを獲得できた

                //デバッグ用 獲得したアイテム名をコンソールに出力
                Debug.Log(itemDataBase.Items[ItemID].ItemName + "を取得");

                //アイテム名を削除する
                //m_gameManager.GetSearchUI().SearchUI_Off();

                //Aボタン表示を暗くする
                //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
                //    "", false);

                //自身を削除する
                Destroy(gameObject);
            }
            else
            {
                //アイテム欄がいっぱいだった

                //デバッグ用
                Debug.Log("アイテム欄がいっぱいです");

                //アイテムがいっぱいなので拾えない
                //m_gameManager.GetSearchUI().SearchUI_On("これ以上アイテムを持てません！", true);
                //m_gameManager.GetSearchUI().AutoOff();

                //効果音再生
                //GameManager.PlaySE(m_gameManager.GetEnterSE());
            }
        }
    }

    //アイテムを捨てた時の処理
    public void ItemDrop(Vector3 playerVelocity)
    {
        //リジッドボディの取得
        Rigidbody rb = GetComponent<Rigidbody>();
        //物理演算を有効にする（？？？？？）
        rb.isKinematic = false;
        //カメラの前方方向に飛ばす
        rb.AddForce((Camera.main.transform.forward * ITEMDROP_POWER) + playerVelocity, ForceMode.Impulse);
        //ランダムに回転
        rb.AddTorque(Random.onUnitSphere * Random.Range(-ITEMDROP_TORQUE, ITEMDROP_TORQUE));

        //しばらく調べられないようにする
        IsCheck = true;
        //1秒後に調べられるようにする
        Invoke("CheckWait", 1.0f);
    }

    //Involeで呼び出す関数
    void CheckWait()
    {
        IsCheck = false;
    }

    //アイテムを使用した時の仮想関数
    public virtual void ItemUse()
    {

    }

    //自分にカーソルがあった時
    public virtual void StartSelect()
    {
        //調べられない状態
        if (IsCheck)
        {
            return;
        }

        //アウトラインを表示する
        m_outline.enabled = true;

        //アイテム名を表示
        string name;
        if (ItemID == -1)
        {
            //調べる系のアイテムなので指定した名前を使う
            name = Name;
            //操作説明の更新
            //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
            //    "調べる", true);
        }
        else
        {
            //獲得できるアイテムなのでアイテムデータベースから名前を引っ張ってくる
            name = m_gameManager.GetItemData().Items[ItemID].ItemName;
            //操作説明の更新
            //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
            //    "拾う", true);
        }
        //m_gameManager.GetSearchUI().SearchUI_On(name, false);
    }

    //自分がカーソルから外れた時
    public virtual void EndSelect()
    {
        //アウトラインを削除する
        m_outline.enabled = false;

        //アイテム名を非表示にする
        //m_gameManager.GetSearchUI().SearchUI_Off();

        //操作説明の更新
        //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
        //     "", false);
    }

    //自分に何かが衝突した瞬間
    private void OnCollisionEnter(Collision collision)
    {
        //自分がアイテム出ないなら中断
        if (gameObject.CompareTag("Item") == false)
        {
            return;
        }

        //重力適応中のみ衝突音を再生
        if (GetComponent<Rigidbody>().isKinematic == false)
        {
            //GameManager.PlaySE(m_gameManager.GetHitSE(),
            //    gameObject,
            //    1.0f, 1.0f);
        }
    }
}
