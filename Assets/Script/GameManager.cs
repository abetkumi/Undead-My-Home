using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player m_player;
    [SerializeField] RectTransform m_canvasRect;
    [SerializeField] GameObject m_machete;
    [SerializeField] GameObject m_pauseObject;
    [SerializeField] Button m_focusButton_Title;

    //効果音再生関数
    static public OneShotAudioClip PlaySE(AudioClip clip,
        float volume = 1.0f,
        float pitch = 1.0f,
        float spatialBlend = 0.0f,
        float minDistance = 0.0f,
        float maxDistance = 0.0f,
        GameObject sauceObject = null)
    {
        //効果音オブジェクトを設定
        GameObject oneShotObj = Instantiate((GameObject)Resources.Load("OneShotSE"));

        //座標を設定
        if (sauceObject != null)
        {
            oneShotObj.transform.position = sauceObject.transform.position;
        }

        //オーディオクリップを設定
        OneShotAudioClip oneShotAudio = oneShotObj.GetComponent<OneShotAudioClip>();
        oneShotAudio.PlaySE(clip, volume, pitch,
            spatialBlend, minDistance, maxDistance);

        return oneShotAudio;
    }

    //ゲームの状態
    public enum GameState
    {
        enGameState_Play,
        enGameState_GameOver,
        enGameState_GameClear,
        enGameState_Shopping,
        enGameState_Pause,
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

    //アイテム
    enum UseItemState
    {
        Machete = 7,
        FireCracker = 8,
        Recovery = 9,
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
    public void SetItemID(int no, int setno)
    {
        ItemID[no] = setno;
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

    //アイテムドロップできるか
    bool m_isItemDrop = true;
    public bool GetItemDrop()
    {
        return m_isItemDrop;
    }
    public void SetItemDrop(bool drop)
    {
        m_isItemDrop = drop;
    }

    //所持金額
    [SerializeField]
    float m_money;
    public float GetMoney()
    {
        return m_money;
    }
    public void SetMoney(float money)
    {
        m_money += money;
    }

    //ノルマ金額
    [SerializeField]
    float m_norma = 100.0f;
    public float GetNorma()
    {
        return m_norma;
    }
    public void SetNorma(float norma)
    {
        m_norma = norma;
    }

    //クリアカウントが3つ貯まるとクリア
    int m_clearCondition = 3;
    public int GetClearCondition()
    {
        return m_clearCondition;
    }

    [SerializeField] int m_clearCount = 0;
    public int GetClearCount()
    {
        return m_clearCount;
    }
    public void SetClearCount(int clearCount)
    {
        m_clearCount = clearCount;
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

                //アイテムを取得時にそのアイテムの重量分加算する。
                m_player.ItemWeightAdd(Item_Data.Items[getItemID].weight, true);

                //効果音再生
                PlaySE(ItemGetSE,0.3f);
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
        if (ItemID[SelectItemNo] == -1 || m_isItemDrop == false)
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
        itemPos.y -= 1.0f;
        GameObject dropItem = Instantiate(Item_Data.Items[ItemID[SelectItemNo]].ItemPrefab,
            itemPos, Camera.main.transform.rotation);

        //テキスト生成
        dropItem.GetComponent<UI_SearchCreater>().m_canvasRect = m_canvasRect;

        //前方に発射
        dropItem.GetComponent<ItemObject>().ItemDrop(velocity);

        //アイテムを捨てる場合、そのアイテムの重量分減算する。
        m_player.ItemWeightAdd(Item_Data.Items[ItemID[SelectItemNo]].weight, false);

        //アイテム欄のIDをリセット
        ItemID[SelectItemNo] = -1;

        //UIを更新
        ItemUI.UpdateUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        //初期ノルマ
        m_norma = 100.0f;
        //UIを更新
        ItemUI.UpdateUI();
        //ステートの更新（初期化）
        m_gameState = GameState.enGameState_Play;
    }

    //マチェーテを使った
    async void UseMachete()
    {
        m_player.Attack();

        //アイテムの重量分減算する。
        m_player.ItemWeightAdd(Item_Data.Items[ItemID[SelectItemNo]].weight, false);

        await UniTask.Delay(1000);
        //アイテム欄のIDをリセット
        ItemID[SelectItemNo] = -1;
        //UIを更新
        ItemUI.UpdateUI();
    }

    //爆竹を使った
    void UseFireCracker()
    {
        //使うアイテムを生成
        Vector3 itemPos = Camera.main.transform.position;
        itemPos.y -= 0.7f;
        GameObject dropItem = Instantiate(Item_Data.Items[ItemID[SelectItemNo]].ItemPrefab,
            itemPos, Camera.main.transform.rotation);
        //前方に発射
        dropItem.GetComponent<ItemObject>().ItemDrop(m_player.transform.position);

        ItemFireCracker fireCracker = dropItem.GetComponent<ItemFireCracker>();
        fireCracker.Fire();
        //アイテムの重量分減算する。
        m_player.ItemWeightAdd(Item_Data.Items[ItemID[SelectItemNo]].weight, false);
        //アイテム欄のIDをリセット
        ItemID[SelectItemNo] = -1;
        //UIを更新
        ItemUI.UpdateUI();
    }

    //回復アイテムを使った
    void UseRecovery()
    {
        if (m_player.GetPlayerHP() >= 100.0f)
        {
            return;
        }
        m_player.RecoveryHP(40.0f);

        //アイテムの重量分減算する。
        m_player.ItemWeightAdd(Item_Data.Items[ItemID[SelectItemNo]].weight, false);
        //アイテム欄のIDをリセット
        ItemID[SelectItemNo] = -1;
        //UIを更新
        ItemUI.UpdateUI();
    }

    void Pause()
    {
        m_gameState = GameState.enGameState_Pause;
        m_pauseObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0.0f;

        EventSystem.current.SetSelectedGameObject(null);
        m_focusButton_Title = m_focusButton_Title.GetComponent<Button>();
        m_focusButton_Title.Select();
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
        if (Input.GetButtonDown("ItemDrop"))
        {
            ItemDrop();
        }

        //現在の所持金とノルマを表示
        if (m_clearCount < m_clearCondition)
        {
            GetOperationUI().SetOperation(UI_Operation.Button.enMoney,
                    "$ " + GetMoney() + "/ $ " + GetNorma(), true);
        }
        else
        {
            GetOperationUI().SetOperation(UI_Operation.Button.enMoney,
                "$ " + GetMoney() + "/ Clear", true);
        }

        //選択されたアイテムを手に持つ
        int selectID = GetItemID(GetSelectItemNo());

        if (selectID == (int)UseItemState.Machete)
        {
            m_machete.SetActive(true);
        }
        else
        {
            m_machete.SetActive(false);
        }

        //選択されたアイテムを使用する
        if (Input.GetButtonDown("UseItem"))
        {

            switch (selectID)
            {
                case (int)UseItemState.Machete:
                    UseMachete();
                    Debug.Log("アタック");
                    return;
                case (int)UseItemState.FireCracker:
                    UseFireCracker();
                    return;
                case (int)UseItemState.Recovery:
                    UseRecovery();
                    return;
                default:
                    Debug.Log("使えるアイテムはない");
                    return;
            }
        }

        //選択アイテムの変更
        if ((Input.GetKeyDown("joystick button 5") || Input.GetAxis("Mouse ScrollWheel") < 0))
        {
            SelectItemNo++;
            if (SelectItemNo > ItemID.Length - 1)
            {
                SelectItemNo = 0;
            }
            //UIを更新
            ItemUI.UpdateUI();
            //効果音再生
            PlaySE(SelectSE, 0.3f);
        }
        if ((Input.GetKeyDown("joystick button 4") || Input.GetAxis("Mouse ScrollWheel") > 0))
        {
            SelectItemNo--;
            if (SelectItemNo < 0)
            {
                SelectItemNo = ItemID.Length - 1;
            }
            //UIを更新
            ItemUI.UpdateUI();
            //効果音再生
            PlaySE(SelectSE, 0.3f);
        }

        if(Input.GetButtonDown("Pause"))
        {
            Pause();
        }
    }
}
