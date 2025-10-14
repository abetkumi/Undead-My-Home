using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] AttackCollider m_attackCollider;
    int m_targetNum = 0;
    bool m_targetMode = false;

    [SerializeField] float m_hp;

    const float CHASE_RANGE = 120.0f;
    const float ATTACK_RANGE = 30.0f;

    [SerializeField]
    float m_searchAngle, m_searchRayRange, m_chaseRayRange;

    //�f�o�b�N�p�ϐ��B
    //���S���ɑS�Ă̏������~������
    bool DebugStop = false;

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
        if (DebugStop == true){
            return;
        }

        Vector3 playerPos = m_targetPlayer.transform.position;
        if (PlayerSearch(m_searchRayRange)){
            if ((transform.position - playerPos).sqrMagnitude <= ATTACK_RANGE){
                m_enemyState = EnemyState.enEnemyState_Attack;
            }
            else{
                m_enemyState = EnemyState.enEnemyState_Chase;
            }
        }
        else{
            m_enemyState = EnemyState.enEnemyState_Lost;
        }
        
        if (Input.GetButton("testKye1")){
            m_enemyState = EnemyState.enEnemyState_Attack;
        }
        else if (Input.GetButton("Jump")){
            TakeDamage(10.0f, 0);
        }

        switch (m_enemyState){
            //����B
            case EnemyState.enEnemyState_Search:
                m_animator.SetBool("Search", true);
                break;
            //�ǐՁB
            case EnemyState.enEnemyState_Chase:
                m_animator.SetBool("Chaes", true);
                m_animator.SetBool("Search", false);
                m_animator.ResetTrigger("Attack");
                break;
            //�������B
            case EnemyState.enEnemyState_Lost:
                m_animator.SetTrigger("Lost");
                m_animator.SetBool("Chaes", false);
                break;
            //�U���B
            case EnemyState.enEnemyState_Attack:
                m_animator.SetTrigger("Attack");
                StartAttack();
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
                m_animator.SetBool("Move", false);
                m_animator.SetTrigger("Death");
                break;
            //����ȊO�B
            default:
                break;
        }
    }

    // �v���C���[��T�� ��������true��Ԃ�
    bool PlayerSearch(float rayRange)
    {
        // ���C�̎n�_���v�Z
        Vector3 startPos = transform.position;
        startPos.y += 10.0f;
        // �v���C���[�֐L�т�x�N�g�����v�Z
        Vector3 diff = m_targetPlayer.transform.position - startPos;

        // ���C��`��
        Debug.DrawRay(startPos, diff.normalized * rayRange, Color.red, 0.1f);

        // ���C�𔭎�
        RaycastHit hit;
        if (Physics.Raycast(startPos, diff.normalized, out hit, rayRange))
        {
            // �v���C���[������p�������C���ŏ��Ƀq�b�g�����̂��v���C���[��������c
            if (Vector3.Angle(transform.forward, diff) <= m_searchAngle
                && hit.collider.CompareTag("Player"))
            {
                // �v���C���[����
                return true;
            }
        }
        return false;
    }

    void StartAttack()
    {
        m_attackCollider.SwitchWnabled(true);
        Invoke("EndAttack", 3.0f);          //3�b��ɔ���������B
    }
    void EndAttack()
    {
        m_attackCollider.SwitchWnabled(false);
    }

    public void TakeDamage(float damage,int damageLevel)
    {
        m_hp -= damage;
        if (damageLevel == 0){
            m_enemyState = EnemyState.enEnemyState_Damage;
        }
        else if (damageLevel == 1){
            m_enemyState = EnemyState.enEnemyState_Stun;
        }

        if (m_hp <= 0){
            m_enemyState=EnemyState.enEnemyState_Death;
            DebugStop = true;
        }
    }
}
