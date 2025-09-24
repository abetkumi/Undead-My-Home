using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    //効果音再生関数
    //static public OneShotAudioClip PlaySE(AudioClip clip,
    //    GameObject sauceObject = null,
    //    float volume = 1.0f,
    //    float spatialBlend = 0.0f,
    //    float minDistance = 0.0f,
    //    float maxDistance = 0.0f)
    //{
    //    //効果音オブジェクトを設定
    //    GameObject oneShotObj = Instantiate((GameObject)Resources.Load("OneShotSE"));

    //    //座標を設定
    //    if (sauceObject != null)
    //    {
    //        oneShotObj.transform.position = sauceObject.transform.position;
    //    }

    //    //オーディオクリップを設定
    //    OneShotAudioClip oneShotAudio = oneShotObj.GetComponent<OneShotAudioClip>();
    //    oneShotAudio.PlaySE(clip, volume,
    //        spatialBlend, minDistance, maxDistance);

    //    return oneShotAudio;
    //}

    //ゲームの状態
    public enum GameState
    {
        enGameState_Play,
        enGameState_Clear,
    }
    static GameState m_gameState = GameState.enGameState_Play;

    public void SetGameState(GameState gameState)
    {
        m_gameState = gameState;
    }
    //どこからでも呼び出せる関数
    public static GameState GetGameState()
    {
        return m_gameState;
    }

    //アイテムデータ
    [SerializeField] ItemData Item_Data;
    public ItemData GetItemData()
    {
        return Item_Data;
    }

    //選択中のアイテム番号（アイテム欄配列の番号）
    [SerializeField] int SelectItemNo = 0;
    public int GetSelectItemNo()
    {
        return SelectItemNo;
    }

    //アイテム欄
    [SerializeField] int[] ItemID;
    //引数番スロットのアイテムを取得
    public int GetItemID(int no)
    {
        return ItemID[no];
    }

    //アイテム欄のUI
    [SerializeField]
    UI_Item ItemUI;

    //アイテム名表示のUI
    [SerializeField]
    UI_Search SearchUI;
    public UI_Search GetSearchUI()
    {
        return SearchUI;
    }


    //操作説明のUI
    [SerializeField]
    UI_Operation OperationUI;
    public UI_Operation GetOperationUI()
    {
        return OperationUI;
    }

    //効果音
    [SerializeField]
    AudioClip ItemGetSE, ItemHitSE, SelectSE, EnterSE;
    public AudioClip GetHitSE()
    {
        return ItemHitSE;
    }
    public AudioClip GetEnterSE()
    {
        return EnterSE;
    }

    //アイテムを取得する
    //アイテム欄に空きがあったらtrue なかったらfalseを返す
    public bool GetItem(int getItemID)
    {
        int selectID = SelectItemNo;

        //順番にアイテム欄を確認していって、空いている場所にIDを格納
        for (int i = 0; i < ItemID.Length; i++)
        {
            if (ItemID[selectID] == -1)
            {
                //空きがあるのでアイテムIDを格納
                ItemID[selectID] = getItemID;
                //効果音再生
                //PlaySE(ItemGetSE);
                //UIを更新
                ItemUI.UpdateUI();

                return true;
            }

            selectID++;

            if (selectID > ItemID.Length - 1)
            {
                //オーバーしたので0に戻す
                selectID = 0;
            }
        }

        //空きがなかった
        return false;
    }

    //引数番スロットのアイテムを捨てる
    void ItemDrop()
    {
        //アイテムがあるか確認
        if (ItemID[SelectItemNo] == -1)
        {
            Debug.Log("【エラー】" + SelectItemNo + "番にアイテムがありません！");
            return;
        }

        //プレイヤーの移動量を取得
        Rigidbody playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        Vector3 velocity = playerRb.velocity;
        velocity.y = 0.0f;

        //捨てるアイテムを生成
        Vector3 itemPos = Camera.main.transform.position;
        GameObject dropItem = Instantiate(Item_Data.Items[ItemID[SelectItemNo]].ItemPrefab,
            itemPos, Camera.main.transform.rotation);
        //前方に発射
        dropItem.GetComponent<ItemObject>().ItemDrop(velocity);

        //アイテム欄のIDをリセット
        ItemID[SelectItemNo] = -1;

        //UIを更新
        ItemUI.UpdateUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        //UIを更新
        ItemUI.UpdateUI();
        //ステートの更新（初期化）
        m_gameState = GameState.enGameState_Play;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイ中でないなら中断
        if (m_gameState != GameState.enGameState_Play)
        {
            return;
        }

        //Bボタンで捨てる
        if ((Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.G)))
        {
            ItemDrop();
        }

        //選択アイテムの変更
        if ((Input.GetKeyDown("joystick button 4") || Input.GetAxis("Mouse ScrollWheel") > 0))
        {
            SelectItemNo++;
            if (SelectItemNo > ItemID.Length - 1)
            {
                SelectItemNo = 0;
            }
            //UIを更新
            ItemUI.UpdateUI();
            //効果音再生
            //PlaySE(SelectSE);
        }
        if ((Input.GetKeyDown("joystick button 5") || Input.GetAxis("Mouse ScrollWheel") < 0))
        {
            SelectItemNo--;
            if (SelectItemNo < 0)
            {
                SelectItemNo = ItemID.Length - 1;
            }
            //UIを更新
            ItemUI.UpdateUI();
            //効果音再生
            //PlaySE(SelectSE);
        }
    }
}
