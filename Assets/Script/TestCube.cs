using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    GameObject[] cubes = new GameObject[100];
    bool[] PointUes = new bool[100];
    //[SerializeField] GameObject Point;
    //GameObject[] PointList = new GameObject[100];

    // Start is called before the first frame update
    void Start()
    {
        
        Transform parent = GameObject.Find("PointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("PointList ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½");
            return;
        }

        int i = 0;
        bool found = false;

        for(int cubeNo = 1; cubeNo < 4; cubeNo++){
            cubes[cubeNo] = GameObject.Instantiate(cubePrefab);

            while (!found){

                i = Random.Range(1, 4);
                //i = cubeNo;

                if (!PointUes[i])
                {
                    string name = "Point" + i.ToString("D3");
                    Transform obj = parent.Find(name);

                    if (obj != null)
                    {
                        cubes[cubeNo].transform.position = obj.transform.position;
                        PointUes[i] = true;
                        found = true;
                    }
                }
                else return;
            }

            found = false;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
