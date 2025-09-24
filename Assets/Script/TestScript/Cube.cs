using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Cube : MonoBehaviour
{
    //キューブのプレハブを指定。
    [SerializeField] GameObject cubePrefab;
    GameObject[] cubes = new GameObject[100];

    //ポイントリストを指定。
    [SerializeField] GameObject PointList;
    Point[] point = new Point[100];

    public List<GameObject> colorPrefabs;

    //キューブの個数。
    int cubeNum = 0;
    //設置したポイントの個数。
    int pointNum = 0;

    Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        SetCube();
        SetPoint();

        PointSelect(cubeNum);
    }

    //キューブの個数設定。
    void SetCube()
    {
        cubeNum = 2;

        for (int i = 0; i < cubeNum; i++){
            cubes[i] = GameObject.Instantiate(cubePrefab);
        }
    }

    //ポイントの個数設定。
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

        while (!found){
            string name = "Point" + (pointNum + 1).ToString("D3");
            Transform obj = parent.Find(name);
            if (obj != null)
            {
                point[pointNum] = new Point();
                pointNum++;
            }
            else found = true;
        }
    }

    //ランダムにPrefabの見た目を変更。
    void SetPrefab(int cubeNo)
    {
        if (colorPrefabs.Count == 0){
            Debug.LogWarning("プレハブが登録されてません！");
            return;
        }

        int index=Random.Range(0, colorPrefabs.Count);
        GameObject obj = Instantiate(colorPrefabs[index], cubes[cubeNo].transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(20f, 20f, 20f);
        cubes[cubeNo] = obj;
    }

    //ポイントを選択。(引数には設置するcubeの数を代入)
    void PointSelect(int count)
    {
        //リスト内の未使用のポイントを検索。
        for (int i = 0; i < count; i++){
            bool found = false;

            while (!found){
                int pointNo = Random.Range(1, pointNum + 1);

                if (!point[pointNo - 1].GetUes()){
                    string name = "Point" + pointNo.ToString("D3");
                    Transform obj = parent.Find(name);

                    if (obj != null)
                    {
                        SetPrefab(i);
                        cubes[i].transform.position = obj.transform.position;
                        point[pointNo - 1].SetUesTrue();
                        found = true;
                    }
                }
                else{}
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
