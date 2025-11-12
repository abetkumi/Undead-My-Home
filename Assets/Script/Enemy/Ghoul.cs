using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghoul : EnemyBase
{
    [SerializeField]
    float m_searchRayRange, m_chaseRayRange;

    const float CHASE_RANGE = 120.0f;
    const float ATTACK_RANGE = 30.0f;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_animator = GetComponent<Animator>();

        m_animator.SetBool("Move", true);
    }

    // Update is called once per frame
    new void Update()
    {
        if (DebugStop == true)
        {
            return;
        }

        if (m_stateLook == true)
        {
            UpdateState();
            return;
        }

        if (PlayerSearch(m_searchRayRange))
        {
            Vector3 playerPos = m_targetPlayer.transform.position;
            m_NextMovePos = playerPos;

            if ((transform.position - m_NextMovePos).sqrMagnitude <= ATTACK_RANGE)
            {
                m_enemyState = EnemyState.enEnemyState_Attack;
            }
        }
        else if ((transform.position - m_NextMovePos).sqrMagnitude != 0.0f)
        {
            m_enemyState = EnemyState.enEnemyState_Chase;
        }
        else
        {
            m_enemyState = EnemyState.enEnemyState_Lost;
        }

        if (Input.GetButton("testKye1"))
        {
            m_navActive = true;
        }
        else if (Input.GetButton("Jump"))
        {
            TakeDamage(10.0f, 0);
        }

        if (m_navActive) { SetNavMovePos(); }

        UpdateState();
        base.Update();
    }

    public override void UpdateState()
    {
        m_animator.ResetTrigger("Attack");
        m_animator.ResetTrigger("ChaesStart");
        m_animator.ResetTrigger("Lost");

        switch (m_enemyState)
        {
            //巡回。
            case EnemyState.enEnemyState_Search:
                m_animator.SetBool("Search", true);
                Move();
                break;
            //追跡。
            case EnemyState.enEnemyState_Chase:
                m_animator.SetBool("Chaes", true);
                m_animator.SetBool("Search", false);
                m_animator.ResetTrigger("Attack");
                m_animator.SetTrigger("ChaesStart");
                Move();
                break;
            //見失う。
            case EnemyState.enEnemyState_Lost:
                m_animator.SetTrigger("Lost");
                m_animator.SetBool("Search", false);
                m_animator.SetBool("Chaes", false);
                break;
            //攻撃。
            case EnemyState.enEnemyState_Attack:
                m_animator.SetBool("Chaes", false);
                m_animator.SetTrigger("Attack");
                StartAttack();
                Move();
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
                m_animator.SetBool("Move", false);
                m_animator.SetTrigger("Death");
                DebugStop = true;
                break;
            //それ以外。
            default:
                break;
        }
    }

    public override void StartAttack()
    {
        m_NextMovePos = transform.position;
        m_attackCollider.SwitchWnabled(true);
        m_stateLook = true;

        if (AnimationEndCheak("Attack") == true)
        {
            EndAttack();
        }
    }

    public override void EndAttack()
    {
        m_attackCollider.SwitchWnabled(false);
        m_stateLook = false;
        m_animator.SetBool("Chaes", true);
        m_animator.SetTrigger("ChaesStart");
    }
}
