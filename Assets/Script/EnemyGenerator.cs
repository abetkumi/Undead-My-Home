using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject GatePrefabs;
    private Point[] spawnPoints;
    private Transform[] pointPos;
    Transform parent;
    private List<GameObject> activeEnemies = new List<GameObject>();

    [SerializeField] private string m_pointListName;
    [SerializeField] private int m_enemyNum;

    [SerializeField] private bool m_startSpawn = false;
    // Start is called before the first frame update
    void Start()
    {
        SetPoint();

        if (m_startSpawn)
            SpawnEnemyStart();
    }

    public void SpawnEnemyStart()
    {
        for (int i = 0; i < m_enemyNum; i++)
            SpawnEnemy();
    }

    private void SpawnGate(Vector3 pos, Quaternion rot)
    {
        GameObject gate = Instantiate(GatePrefabs, pos, rot);
        Destroy(gate, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButton("Jump")) SpawnEnemy();
    }

    private void SetPoint()
    {
        //ポイントリストが存在するか検索。
        parent = transform;
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
            string name = m_pointListName + (i + 1).ToString("D3"); // 001形式
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

    private void SpawnEnemy()
    {
        //ランダムにエネミーを選択。
        int enemyIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject prefab = enemyPrefabs[enemyIndex];

        //ランダムに出現場所を選択。
        int pointIndex = Random.Range(0, pointPos.Length);
        Transform spawnPoint = pointPos[pointIndex].transform;

        SpawnGate(spawnPoint.position, spawnPoint.rotation);

        GameObject enemy = Instantiate(prefab, spawnPoint.transform);

        activeEnemies.Add(enemy);
    }
}