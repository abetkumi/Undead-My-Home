using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    private Point[] spawnPoints;
    private GameObject[] pointPos;
    Transform parent;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump"))
            SpawnEnemy();
    }

    //void SetPoint()
    //{
    //    bool found = false;
    //    int count = 0;

    //    //ポイントリストが存在するか検索。
    //    parent = GameObject.Find("EnemySpawnPointList")?.transform;
    //    if (parent == null)
    //    {
    //        Debug.LogWarning("PointList が見つかりませんでした");
    //        return;
    //    }

    //    while (!found)
    //    {
    //        string name = "SpawnPoint" + (count + 1).ToString("D3");
    //        Transform obj = parent.Find(name);
    //        if (obj != null)
    //        {
    //            Point p = obj.GetComponent<Point>();
    //            if (p != null)
    //            {
    //                spawnPoints[count] = p;
    //                pointPos[count].transform.position = obj.transform.position;
    //                count++;
    //            }
    //            else
    //            {
    //                Debug.LogWarning($"{name} に Point コンポーネントがありません");
    //                found = true; // 終了するか、スキップするかは要検討。
    //            }
    //        }
    //        else found = true;
    //    }
    //}
    void SetPoint()
    {
        bool found = false;
        int count = 0;

        //ポイントリストが存在するか検索。
        parent = GameObject.Find("EnemySpawnPointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("PointList が見つかりませんでした");
            return;
        }

        while (!found)
        {
            string name = "SpawnPoint" + (count + 1).ToString("D3");
            Transform obj = parent.Find(name);
            if (obj != null)
            {
                pointPos[count] = new GameObject("Point" + (count + 1));
                pointPos[count].transform.position = obj.transform.position;
                count++;
            }
            else found = true;
        }
    }

    public void SpawnEnemy()
    {
        //ランダムにエネミーを選択。
        int enemyIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject prefab = enemyPrefabs[enemyIndex];

        //ランダムに出現場所を選択。
        int pointIndex = Random.Range(0, pointPos.Length);
        Transform spawnPoint = pointPos[pointIndex].transform;

        GameObject enemy = Instantiate(prefab, spawnPoint.transform);

        activeEnemies.Add(enemy);
    }
}