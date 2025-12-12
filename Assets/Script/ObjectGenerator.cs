using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public List<GameObject> objPrefabs;
    GameObject[] m_objects = new GameObject[100];

    //ポイントリストを指定。
    Point[] point = new Point[100];

    Transform parent;

    //オブジェクトの最小個数、最大個数、合計金額の目安、オブジェクトを置けるポイントの数。。
    private int
        m_minObjCount, m_maxObjCount, m_targetPrice, m_basePoint;
    //、許容誤差範囲。
    private float m_toleranceRate;

    private List<int> prices = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        SetPoint();

        GenerateObjects();
    }

    void GenerateObjects()
    {
        List<int> generated = new List<int>();
        int totalPrice = 0;

        //個数をランダムに設定。
        int count = Random.Range(m_minObjCount, m_maxObjCount + 1);

        if (count > m_basePoint)
        {
            Debug.LogWarning($"⚠ 警告: 土台容量({m_basePoint})を超えています！");
            return;
        }
    }

    void SetPoint()
    {
        bool found = false;

        //ポイントリストが存在するか検索。
        parent = GameObject.Find("PointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("PointList が見つかりませんでした");
            return;
        }

        while (!found)
        {
            string name = "Point" + (m_basePoint + 1).ToString("D3");
            Transform obj = parent.Find(name);
            if (obj != null)
            {
                Point p = obj.GetComponent<Point>();
                if (p != null)
                {
                    point[m_basePoint] = p;
                    m_basePoint++;
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

    void SetObjectsPrice()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    /////////////////////////////////////////////////
    //外部からのアクセス。
    /////////////////////////////////////////////////

    //オブジェクトの最小、最大個数を設定。
    public void SetObjCount(int min, int max)
    {
        m_minObjCount = min;
        m_maxObjCount = max;
    }

    //合計金額の目安を設定。
    public void SetTargetPrice(int price)
    {
        m_targetPrice = price;
    }

    //許容誤差の設定。
    public void SetToleranceRate(float tolerance)
    {
        m_toleranceRate = tolerance;
    }

    //まとめてセットしたい場合。
    public void AllSet(int min, int max, int price, float tolerance)
    {
        m_minObjCount = min;
        m_maxObjCount = max;
        m_targetPrice = price;
        m_toleranceRate = tolerance;
    }
}
