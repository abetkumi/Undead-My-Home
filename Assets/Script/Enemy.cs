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
        enEnemyState_Search,    //����B
        enEnemyState_Chase,     //�ǐՁB
        enEnemyState_Lost,      //�������B
        enEnemyState_Attack,    //�U���B
        enEnemyState_Escape,    //������B
        enEnemyState_Damage,    //�_���[�W�B
        enEnemyState_Stun,      //�C��B
        enEnemyState_Death,     //���B
        enEnemyState_Num,       //�X�e�[�g�̐��B
    }

    [SerializeField] EnemyState m_enemyState = EnemyState.enEnemyState_Search;
    [SerializeField] Vector3[] m_targetPos;
    int m_targetNum = 0;
    bool m_targetMode = false;
    void TargetAdd(int add)
    {

    }

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        m_animator.SetBool("Move", true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_enemyState){
            //����B
            case EnemyState.enEnemyState_Search:
                m_animator.SetBool("Search", true);
                break;
            //�ǐՁB
            case EnemyState.enEnemyState_Chase:
                m_animator.SetBool("Chaes", true);
                break;
            //�������B
            case EnemyState.enEnemyState_Lost:
                m_animator.SetTrigger("Lost");
                break;
            //�U���B
            case EnemyState.enEnemyState_Attack:
                m_animator.SetTrigger("Attack");
                break;
            //������B
            case EnemyState.enEnemyState_Escape:
                break;
            //�_���[�W�B
            case EnemyState.enEnemyState_Damage:
                m_animator.SetTrigger("Damage");
                break;
            //�C��B
            case EnemyState.enEnemyState_Stun:
                m_animator.SetTrigger("Knockback");
                break;
            //���B
            case EnemyState.enEnemyState_Death:
                m_animator.SetTrigger("Death");
                break;
            //����ȊO�B
            default:
                break;
        }
    }
}
