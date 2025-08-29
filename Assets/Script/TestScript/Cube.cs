using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    //キューブのプレハブを指定。
    [SerializeField] GameObject cubePrefab;
    GameObject[] cubes = new GameObject[100];

    //ポイントリストを指定。
    [SerializeField] GameObject PointList;
    Point[] point = new Point[100];

    //キューブの個数。
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetCount();

        for(int i = 1; i <= count; i++){
            cubes[i] = GameObject.Instantiate(cubePrefab);
            point[i] = PointList.GetComponent<Point>();
        }
    }

    //キューブの個数設定。
    void SetCount()
    {
        count = 3;
    }

    //ポイントを選択。(引数には設置するcubeの数を代入)
    void PointSelect(int count)
    {
        //ポイントリストが存在するか検索。
        Transform parent = GameObject.Find("PointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("PointList が見つかりませんでした");
            return;
        }

        //リスト内の未使用のポイントを検索。
        for (int i = 1; i <= count; i++){
            cubes[i] = GameObject.Instantiate(cubePrefab);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
