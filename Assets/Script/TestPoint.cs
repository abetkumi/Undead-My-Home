using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoint : MonoBehaviour
{
    [SerializeField] GameObject cubeObject;
    TestCube cube;

    
    // Start is called before the first frame update
    void Start()
    {
        cube = cubeObject.GetComponent<TestCube>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
