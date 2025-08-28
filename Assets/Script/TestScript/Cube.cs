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
    GameObject[] point = new GameObject[100];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
