using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public Transform animatedBone;
    public Collider hitBox;

    public float damage = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        SwitchWnabled(false);
    }

    // Update is called once per frame
    void Update()
    {
        hitBox.transform.position = animatedBone.transform.position;
        hitBox.transform.rotation = animatedBone.transform.rotation;
    }

    public void SwitchWnabled(bool i)
    {
        hitBox.enabled = i;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerAvoid player = other.GetComponent<PlayerAvoid>();
        if (player != null)
        {
            if (player.IsInvincible())
            {
                return;
            }
            //プレイヤー側の処理
            if (other.CompareTag("Player"))
            {
                Player hp = other.GetComponent<Player>();
                if (hp != null)
                {
                    hp.TakeDamage(damage);
                    return;
                }
            }
            //エネミー側の処理
            else if (other.CompareTag("Enemy"))
            {
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, 1);
                    return;
                }
            }
        }
    }
}