using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFireHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        enemy.TakeDamage(0, 1);
    }
}
