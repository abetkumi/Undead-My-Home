using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アイテム用データベース
[CreateAssetMenu(fileName = "itemDataBase", menuName = "CreateItemDataBase")]
public class ItemData : ScriptableObject
{
    //アイテム情報の構造体
    [System.Serializable]
    public struct Item
    {
        public string ItemName; //アイテム名
        [Multiline(2)]
        public string ItemExplanation; //アイテム欄で表示する説明文
        public GameObject ItemPrefab;

        //設置用パラメータ
        public Vector3 ItemPivot;   //アイテムを置いたときに座標を補正する量

        //UI用パラメータ
        public Vector3 UI_Offset;
        public Vector3 UI_Rotation;
        public Vector3 UI_Scale;

        //重さ。(単位はkg)
        public float weight;
        //価値。
        public float value;

    }

    //アイテムリストの可変長配列
    public Item[] Items;
}
