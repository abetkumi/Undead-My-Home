using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    private Point[] spawnPoints;
    private Transform[] pointPos;
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
        //if (Input.GetButton("Jump")) SpawnEnemy();
    }

    void SetPoint()
    {
        //ポイントリストが存在するか検索。
        parent = GameObject.Find("EnemySpawnPointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("PointList が見つかりませんでした");
            return;
        }

        // 子オブジェクト数を取得して配列を確保
        int childCount = parent.childCount;
        pointPos = new Transform[childCount];

        // SpawnPoint001 ～ SpawnPointXXX を順番に探して格納
        for (int i = 0; i < childCount; i++)
        {
            string name = "SpawnPoint" + (i + 1).ToString("D3"); // 001形式
            Transform obj = parent.Find(name);

            if (obj != null)
            {
                pointPos[i] = obj;
            }
            else
            {
                Debug.LogWarning($"{name} が見つかりませんでした");
            }
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