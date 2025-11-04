using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPointList : MonoBehaviour
{
    enum PointListState{
        Normal,
        Room,
        Num,
    }

    Transform[] m_navPointPos = new Transform[100];

    //設置したポイントの個数。
    int pointNum = 0;

    Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        SetPointNum();
    }

    //ポイントの個数設定。
    void SetPointNum()
    {
        bool found = false;

        //ポイントリストが存在するか検索。
        parent = GameObject.Find("NavPointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("NavPointList が見つかりませんでした");
            return;
        }

        while (!found)
        {
            string name = "NavPoint" + (pointNum + 1).ToString("D3");
            Transform obj = parent.Find(name);
            if (obj != null)
            {
                Transform p = obj.GetComponent<Transform>();
                if (p != null)
                {
                    m_navPointPos[pointNum] = p;
                    pointNum++;
                }
                else
                {
                    Debug.LogWarning($"{name} に Point コンポーネントがありません");
                    found = true; // 終了するか、スキップするかは要検討。
                }
            }
            else found = true;
        }
    }

    //ポイントの数を返す。
    public int GetPointNum()
    {
        return pointNum;
    }
    
    //ポイントリストx番の位置情報を取得。
    public Vector3 GetPointPos(int pointNo)
    {
        return m_navPointPos[pointNo].position;
    }
    
    //ポイントリストx行y番の位置情報を取得。
    //引数には１行当たりの配列数、何行目、何番の順番で宣言。
    public Vector3 GetPointPos(int ListSize,int pointListNum,int pointNo)
    {
        int returnPointNo = (ListSize * pointListNum) + pointNo;
        return m_navPointPos[returnPointNo].position;
    }
}
