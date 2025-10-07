using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public Transform animatedBone;
    public Collider hitBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hitBox.transform.position = animatedBone.transform.position;
    }

    public int damage = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player hp = other.GetComponent<Player>();
            if (hp != null)
            {
                //hp.TakeDamage(damage);
            }
        }
    }
}