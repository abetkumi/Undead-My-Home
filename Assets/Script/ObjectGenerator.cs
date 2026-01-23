using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public List<GameObject> objPrefabs;
    GameObject[] m_objects = new GameObject[100];

    //ポイントリストを指定。
    Point[] point = new Point[100];

    [SerializeField] private ItemData m_itemData;
    
    Transform parent;

    //オブジェクトの最小個数、最大個数、オブジェクトを置けるポイントの数。。
    private int
        m_minObjCount, m_maxObjCount, m_basePoint;
    //、合計金額の目安、許容誤差範囲。
    private float m_targetPrice, m_toleranceRate;

    private List<float> prices = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        SetObjectsPrice();
        SetPoint();

        AllSet(20, 30, 500.0f, 0.1f);

        GenerateObjects();
    }

    //void GenerateObjects()
    //{
    //    List<float> generated = new List<float>();
    //    float totalPrice = 0;

    //    //個数をランダムに設定。
    //    int count = Random.Range(m_minObjCount, m_maxObjCount + 1);
    //    m_objects =new GameObject[count];

    //    if (count > m_basePoint)
    //    {
    //        Debug.LogWarning($"⚠ 警告: 土台容量({m_basePoint})を超えています！");
    //        return;
    //    }

    //    // 許容範囲計算
    //    int minAcceptable = Mathf.RoundToInt(m_targetPrice * (1f - m_toleranceRate));
    //    int maxAcceptable = Mathf.RoundToInt(m_targetPrice * (1f + m_toleranceRate));

    //    // 目安金額に近づけるように生成
    //    while (generated.Count < count)
    //    {
    //        float price = prices[Random.Range(0, objPrefabs.Count)];
    //        GameObject prefab = objPrefabs[(int)price];
    //        if (totalPrice + price <= maxAcceptable)
    //        {
    //            SetPrefab(generated.Count, (int)price);
    //            PointSelect(point.Length);

    //            // 仮生成
    //            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
    //            obj.transform.localScale = new Vector3(20f, 20f, 20f);
    //            m_objects[generated.Count] = obj;

    //            // ポイントに配置
    //            PointSelect(count);

    //            generated.Add(price);
    //            totalPrice += price;
    //        }
    //        else
    //        {
    //            // 追加すると上限を超える場合はスキップ
    //            break;
    //        }
    //    }
    //}

    public void GenerateObjects()
    {
        List<float> generated = new List<float>();
        float totalPrice = 0;

        int count = Random.Range(m_minObjCount, m_maxObjCount + 1);
        m_objects = new GameObject[count];

        if (count > m_basePoint)
        {
            Debug.LogWarning($"⚠ 警告: 土台容量({m_basePoint})を超えています！");
            return;
        }

        int minAcceptable = Mathf.RoundToInt(m_targetPrice * (1f - m_toleranceRate));
        int maxAcceptable = Mathf.RoundToInt(m_targetPrice * (1f + m_toleranceRate));

        bool objPriceOver = false;
        int generatedObjCount = 0;
        while (generated.Count < count && !objPriceOver)
        {
            int objNo = Random.Range(1, objPrefabs.Count + 1);
            float price = prices[objNo];
            if (totalPrice + price <= maxAcceptable)
            {
                GameObject prefab = objPrefabs[objNo - 1];

                // 仮生成
                GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                //obj.transform.localScale = new Vector3(20f, 20f, 20f);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                m_objects[generated.Count] = obj;

                generated.Add(price);
                totalPrice += price;
                generatedObjCount++;
            }
            else if (totalPrice < minAcceptable)
            {
                
            }
            else { objPriceOver = true; continue; }
        }
        // ポイントに配置
        PointSelect(generatedObjCount);
        Debug.Log("今回の合計金額は" + totalPrice + "です");
    }

    private void SetPoint()
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

    private void SetObjectsPrice()
    {
        if (m_itemData == null) return;

        // アイテム数を取得
        int itemCount = m_itemData.Items.Length;
        Debug.Log("アイテム数: " + itemCount);

        // 各アイテムの value を List に追加
        foreach (var item in m_itemData.Items)
        {
            prices.Add(item.value);
        }
    }

    //ポイントを選択。(引数には設置するcubeの数を代入)
    private void PointSelect(int count)
    {
        //リスト内の未使用のポイントを検索。
        for (int i = 0; i < count; i++){
            bool found = false;

            while (!found){
                int pointNo = Random.Range(1, m_basePoint + 1);

                if (!point[pointNo - 1].GetUes()){
                    string name = "Point" + pointNo.ToString("D3");
                    Transform obj = parent.Find(name);

                    if (obj != null){
                        m_objects[i].transform.position = obj.transform.position;
                        point[pointNo - 1].SetUesTrue();
                        found = true;
                    }
                }
                else { }
            }
        }
    }

    //オブジェクトの見た目を変更。
    private void SetPrefab(int cubeNo, int objNo)
    {
        if (objPrefabs.Count == 0)
        {
            Debug.LogWarning("プレハブが登録されてません！");
            return;
        }

        if (objPrefabs[objNo] == null)
        {
            Debug.LogError($"colorPrefabs[{objNo}] が null です！");
            return;
        }

        GameObject obj = Instantiate(objPrefabs[objNo], m_objects[cubeNo].transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(20f, 20f, 20f);
        Destroy(m_objects[cubeNo]);
        m_objects[cubeNo] = obj;
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
    public void SetTargetPrice(float price)
    {
        m_targetPrice = price;
    }

    //許容誤差の設定。
    public void SetToleranceRate(float tolerance)
    {
        m_toleranceRate = tolerance;
    }

    //まとめてセットしたい場合。
    public void AllSet(int min, int max, float price, float tolerance)
    {
        m_minObjCount = min;
        m_maxObjCount = max;
        m_targetPrice = price;
        m_toleranceRate = tolerance;
    }
}
