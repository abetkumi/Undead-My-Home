using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject m_targetPlayer;
    [SerializeField] AudioClip StepSE;

    NavMeshAgent m_agent;
    Animator m_animator;

    enum EnemyState{
        enEnemyState_Search,    //巡回。
        enEnemyState_Chase,     //追跡。
        enEnemyState_Lost,      //見失う。
        enEnemyState_Attack,    //攻撃。
        enEnemyState_Escape,    //逃げる。
        enEnemyState_Damage,    //ダメージ。
        enEnemyState_Stun,      //気絶。
        enEnemyState_Death,     //死。
        enEnemyState_Num,       //ステートの数。
    }

    [SerializeField] EnemyState m_enemyState = EnemyState.enEnemyState_Search;
    [SerializeField] Vector3[] m_targetPos;
    [SerializeField] AttackCollider m_attackCollider;
    int m_targetNum = 0;
    bool m_targetMode = false;

    Collider m_SwordCollider;

    void TargetAdd(int add)
    {

    }

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        m_animator.SetBool("Move", true);

        //m_SwordCollider = transform.Find("Object01").GetComponent<Collider>();
        //m_SwordCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("testKye1")){
            m_enemyState = EnemyState.enEnemyState_Attack;
        }
        else if (Input.GetButton("Jump")){
            m_enemyState = EnemyState.enEnemyState_Damage;
        }

            switch (m_enemyState){
            //巡回。
            case EnemyState.enEnemyState_Search:
                m_animator.SetBool("Search", true);
                break;
            //追跡。
            case EnemyState.enEnemyState_Chase:
                m_animator.SetBool("Chaes", true);
                m_animator.SetBool("Search", false);
                break;
            //見失う。
            case EnemyState.enEnemyState_Lost:
                m_animator.SetTrigger("Lost");
                break;
            //攻撃。
            case EnemyState.enEnemyState_Attack:
                m_animator.SetTrigger("Attack");
                StartAttack();
                break;
            //逃げる。
            case EnemyState.enEnemyState_Escape:
                break;
            //ダメージ。
            case EnemyState.enEnemyState_Damage:
                m_animator.SetTrigger("Damage");
                break;
            //気絶。
            case EnemyState.enEnemyState_Stun:
                m_animator.SetTrigger("Knockback");
                break;
            //死。
            case EnemyState.enEnemyState_Death:
                m_animator.SetTrigger("Death");
                break;
            //それ以外。
            default:
                break;
        }
    }

    public void StartAttack()
    {
        //m_SwordCollider.enabled = true;
        Invoke("EndAttack", 1.0f);          //1秒後に判定を消す。
    }
    void EndAttack()
    {
        //m_SwordCollider.enabled = false;
    }

    //public int damage = 10;

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Player hp = other.GetComponent<Player>();
    //        if (hp != null)
    //        {
    //            //hp.TakeDamage(damage);
    //        }
    //    }
    //}
}
