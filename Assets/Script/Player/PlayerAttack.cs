using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーアタック時の処理クラス
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject m_playerAnimationObject;
    [SerializeField] BoxCollider m_attackCollider;
    [SerializeField] AudioClip m_hitSE;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(4, 4, 4);
        m_attackCollider.enabled = false;
    }

    public void NormalScale()
    {
        gameObject.transform.localScale = Vector3.one;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.TakeDamage(1, 1);
            m_attackCollider.enabled = false;
            GameManager.PlaySE(m_hitSE);
            Debug.Log("敵にヒット");
        }
    }
}
