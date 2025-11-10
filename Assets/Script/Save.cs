using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : object
{
    //所持金
    public float m_money;
    //クリア回数
    public int m_clearCount;
    //アイテム欄
    public int[] ItemID;

    

    public void SetMoney(float money)
    {
        m_money = money;
    }

    public void SetClearCount(int clearCount)
    {
        m_clearCount = clearCount;
    }

    public float GetMoney() 
    {
        return m_money; 
    }

    public int GetClearCount()
    {
        return m_clearCount;
    }

    public string GetNormalData()
    {
        return "money: " + m_money + "clearCount: " + m_clearCount;
    }

    public string GetJsonData()
    {
        return JsonUtility.ToJson(this);
    }
}
