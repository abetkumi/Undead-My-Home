using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : EnemyBase
{
    [SerializeField]
    float m_searchRayRange, m_chaseRayRange;

    const float CHASE_RANGE = 120.0f;
    const float ATTACK_RANGE = 30.0f;

    float m_speed = 3.5f;
    [SerializeField] float time = 0.0f;

    float m_dashTime = 0.0f;
    bool m_dashActiv = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_animator = GetComponent<Animator>();
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
            m_NextMovePos = m_targetPlayer.transform.position;

            if ((transform.position - m_NextMovePos).sqrMagnitude <= ATTACK_RANGE && GetCooldown(m_attackCoolTime))
                m_enemyState = EnemyState.enEnemyState_Attack;
            else if ((transform.position - m_NextMovePos).sqrMagnitude != 0.0f)
                m_enemyState = EnemyState.enEnemyState_Chase;

            if (SoundTimer(8.0f))
                PlaySound(0);

            m_dashTime = 0.0f;
            m_dashActiv = true;

            UpdateState();
            return;
        }
        else
        {
            if (m_dashActiv == true) {
                m_dashTime += Time.deltaTime;

                if (m_dashTime >= 0.5f)
                    m_NextMovePos = transform.position;
            }
        }

        //デバック用。
        //if (Input.GetButton("testKye1"))
        //    PlaySound(0);
        //if (Input.GetButton("Jump"))
        //{
        //    ResetAllAnimatorParameters();
        //    m_animator.SetTrigger("SleepStart");
        //    m_enemyState = EnemyState.enEnemyState_Sleep;
        //}

        base.Update();
        if (m_navActive) { SetNavMovePos(); }

        UpdateState();
    }

    public override void UpdateState()
    {
        ResetAllAnimatorParameters();

        switch (m_enemyState)
        {
            //巡回。
            case EnemyState.enEnemyState_Search:
                m_animator.SetFloat("Walk", 1.0f);
                Move();
                break;
            //追跡。
            case EnemyState.enEnemyState_Chase:
                ChaseSpeedSet();
                m_animator.SetFloat("Walk", m_speed);
                Move();
                break;
            //見失う。
            case EnemyState.enEnemyState_Lost:
                m_animator.SetFloat("Walk", 0.0f);
                m_animator.SetTrigger("IdelAction");
                m_speed = 3.5f;
                LostKeepTime(3.0f);
                break;
            //攻撃。
            case EnemyState.enEnemyState_Attack:
                m_animator.SetFloat("Walk", 0.0f);
                m_animator.SetTrigger("Hit");
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
                break;
            //眠る。
            case EnemyState.enEnemyState_Sleep:
                Sleep();
                break;
            //死。
            case EnemyState.enEnemyState_Death:
                m_animator.SetTrigger("Die");
                DebugStop = true;
                break;
            //それ以外。
            default:
                break;
        }
    }

    void ChaseSpeedSet(){
        m_speed += Time.deltaTime * 3.0f;
        m_agent.speed = m_speed;
    }

    void Sleep()
    {
        time += Time.deltaTime;
        m_stateLook = true;

        if (time >= 3.0f) {
            m_stateLook = false;
            m_animator.SetTrigger("SleepEnd");
            time = 0.0f;
        }
    }

    public override void StartAttack()
    {
        m_NextMovePos = transform.position;
        m_attackCollider.SwitchWnabled(true);
        m_stateLook = true;

        if (AnimationEndCheck("Hit") == true)
        {
            EndAttack();
        }
    }

    public override void EndAttack()
    {
        m_attackCollider.SwitchWnabled(false);
        m_stateLook = false;
    }
}