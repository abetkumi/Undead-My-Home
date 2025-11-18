using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ItemRolling : MonoBehaviour
{
    private void Awake()
    {
        transform.rotation = Quaternion.Euler(0, 0, 45);
    }
    // Update is called once per frame
    void Update()
    {
        if(gameObject.layer == LayerMask.NameToLayer("UI_Item"))
        {
            return;
        }
        transform.Rotate(new Vector3(0, 1, 0) * 200f * Time.deltaTime, Space.World);
    }
}
