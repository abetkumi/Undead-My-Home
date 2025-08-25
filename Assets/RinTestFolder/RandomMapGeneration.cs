using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapGeneration : MonoBehaviour
{
    enum MapSelect
    {
        None,
        Map1,
        Map2,
        Map3,
        Num,
    }

    // Start is called before the first frame update
    void Start()
    {
        Random.Range(1,3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
