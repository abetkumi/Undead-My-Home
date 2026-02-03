using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghoul : EnemyBase
{
    [SerializeField]
    float m_searchRayRange, m_chaseRayRange;

    const float CHASE_RANGE = 120.0f;
    const float ATTACK_RANGE = 30.0f;
    enum GhoulSound
    {
        enGhoulSound_voice,
        enGhoulSound_Attack,
        enGhoulSound_Footsteps,
        enGhoulSound_Num,
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
        if (DebugStop)
        {
            return;
        }

        if (m_Stan)
        {
            UpdateState();
            return;
        }

        if (m_stateLook)
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
            else if ((transform.position - m_NextMovePos).sqrMagnitude <= CHASE_RANGE)
                m_enemyState = EnemyState.enEnemyState_Chase;

            if (SoundTimer(m_enemyVoice))
            {
                PlaySound((int)GhoulSound.enGhoulSound_voice);
                SetRandamTimer();
            }
                
            UpdateState();
            return;
        }

        //デバック用。
        //if (Input.GetButton("testKye1"))
        //    m_navActive = true;
        //if (Input.GetButton("Jump"))
        //    TakeDamage(10.0f, 0);

        base.Update();
        if (m_navActive) SetNavMovePos();

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
                Move(1.0f);
                break;
            //追跡。
            case EnemyState.enEnemyState_Chase:
                m_animator.SetBool("Chaes", true);
                m_animator.SetTrigger("ChaesStart");
                Move(1.5f);
                break;
            //見失う。
            case EnemyState.enEnemyState_Lost:
                m_animator.SetTrigger("Lost");
                LostKeepTime(3.0f);
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
                DamageAnimation();
                break;
            //気絶。
            case EnemyState.enEnemyState_Stun:
                m_animator.SetTrigger("Lost");
                StunTimer(m_damageStanTime);
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
        PlaySound((int)GhoulSound.enGhoulSound_Footsteps);
    void PlayAttackSound() =>
        PlaySound((int)GhoulSound.enGhoulSound_Attack);
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
