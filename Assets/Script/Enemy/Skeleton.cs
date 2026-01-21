using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Skeleton : EnemyBase
{
    [SerializeField]
    float m_searchRayRange, m_chaseRayRange;

    const float CHASE_RANGE = 120.0f;
    const float ATTACK_RANGE = 30.0f;

    enum SkeletonSound{
        enSkeletonSound_voice1,
        enSkeletonSound_voice2, 
        enSkeletonSound_Attack,
        enSkeletonSound_Footsteps,
        enSkeletonSound_Num,
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_animator.applyRootMotion = false;

        m_animator.SetBool("Move", true);
    }

    // Update is called once per frame
    new void Update() 
    {
        if (DebugStop) return;

        if (m_stateLook == true)
        {
            UpdateState();
            if (PlayerSearch(m_searchRayRange) && m_enemyState == EnemyState.enEnemyState_Lost) 
                m_stateLook = false;

            return;
        }

        if (PlayerSearch(m_searchRayRange))
        {
            m_NextMovePos = m_targetPlayer.transform.position;

            if ((transform.position - m_NextMovePos).sqrMagnitude <= ATTACK_RANGE && GetCooldown(m_attackCoolTime))
                m_enemyState = EnemyState.enEnemyState_Attack;
            else if ((transform.position - m_NextMovePos).sqrMagnitude < ATTACK_RANGE)
                m_enemyState = EnemyState.enEnemyState_Lost;
            else if ((transform.position - m_NextMovePos).sqrMagnitude <= CHASE_RANGE)
                m_enemyState = EnemyState.enEnemyState_Chase;

            if (SoundTimer(10.0f)){
                if (Random.value < 0.5f)
                    PlaySound((int)SkeletonSound.enSkeletonSound_voice1);
                else PlaySound((int)SkeletonSound.enSkeletonSound_voice2);
            }
                
            UpdateState();
            return;
        }

        //デバック用。
        //if (Input.GetButton("testKye1"))
        //    m_navActive = true;
        //if (Input.GetButtonDown("testKye1"))
        //{
        //    TakeDamage(10, 0);
        //    return;
        //}

        base.Update();
        if (m_navActive) { SetNavMovePos(); }

        UpdateState();
    }

    public override void UpdateState()
    {
        ResetAllAnimatorParameters();
        m_animator.SetBool("Move", true);

        switch (m_enemyState)
        {
            //巡回。
            case EnemyState.enEnemyState_Search:
                m_animator.SetBool("Search", true);
                Move();
                break;
            //追跡。
            case EnemyState.enEnemyState_Chase:
                m_animator.SetBool("Chase", true);
                m_animator.SetTrigger("ChaesStart");
                Move();
                break;
            //見失う。
            case EnemyState.enEnemyState_Lost:
                m_animator.SetTrigger("Lost");
                LostKeepTime(3.0f);
                break;
            //攻撃。
            case EnemyState.enEnemyState_Attack:
                if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    m_animator.SetTrigger("Attack");
                m_stateLook = true;
                StartAttack();
                break;
            //逃げる。
            case EnemyState.enEnemyState_Escape:
                break;
            //ダメージ。
            case EnemyState.enEnemyState_Damage:
                m_animator.SetTrigger("Damage");
                DamageAnimation();
                break;
            //気絶。
            case EnemyState.enEnemyState_Stun:
                m_animator.SetTrigger("Knockback");
                DamageAnimation();
                break;
            //死。
            case EnemyState.enEnemyState_Death:
                m_animator.SetTrigger("Death");
                DebugStop = true;
                break;
            //それ以外。
            default:
                break;
        }
    }

    //アニメーションのイベントにより呼び出し。
    //-----------------------------------------------------------//
    void PlayFootstepsSound() =>
        PlaySound((int)SkeletonSound.enSkeletonSound_Footsteps);

    void PlayAttackSound() =>
        PlaySound((int)SkeletonSound.enSkeletonSound_Attack);
    //-----------------------------------------------------------//


    public override void StartAttack()
    {
        m_stateLook = true;

        if (AnimationEndCheck("Attack") == true)
            EndAttack();
    }

    public override void EndAttack()
    {
        m_stateLook = false;
        m_animator.ResetTrigger("Attack");
        m_enemyState = EnemyState.enEnemyState_Search;
    }
}
