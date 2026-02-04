using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected AttackCollider m_attackCollider;
    [SerializeField] protected Animator m_animator;

    protected GameObject m_targetPlayer;

    [SerializeField] protected AudioClip[] m_soundClip;
    private AudioSource[] m_audioSource;

    protected NavMeshAgent m_agent;
    protected Rigidbody rb;

    private float m_defaultSpeed;

    [SerializeField] protected float m_searchAngle;

    NavPointList m_navPoint;
    int m_currentTarget = -1;
    protected bool m_navActive = false;

    private float stuckThreshold = 0.02f;            //スタック検知可能な移動距離。
    private float stuckTimeRequired = 3.5f;         //スタック時間。

    private Vector3 m_lastPos;
    private float m_stuckTimer;

    [SerializeField] protected Vector3 m_NextMovePos = Vector3.zero;             //次の移動先。

    protected enum EnemyState
    {
        enEnemyState_Search,    //巡回。
        enEnemyState_Chase,     //追跡。
        enEnemyState_Lost,      //見失う。
        enEnemyState_Attack,    //攻撃。
        enEnemyState_Escape,    //逃げる。
        enEnemyState_Damage,    //ダメージ。
        enEnemyState_Stun,      //気絶。
        enEnemyState_Death,     //死。
        enEnemyState_Sleep,     //眠る。
        enEnemyState_Num,       //ステートの数。
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    //インスペクターによる確認用。
    [SerializeField] protected EnemyState m_enemyState = EnemyState.enEnemyState_Search;
    [SerializeField] public bool m_stateLook = false;

    [SerializeField] float m_hp;
    ////////////////////////////////////////////////////////////////////////////////////////////

    float m_attackCooldown = 100.0f;
    [SerializeField] protected float m_attackCoolTime;

    protected bool m_Stan = false;
    private float m_stunTimer = 0.0f;
    protected float m_damageStanTime = 3.0f;

    //デバック用変数。
    //死亡時に全ての処理を停止させる
    protected bool DebugStop = false;

    protected float soundTimer = 100.0f;
    protected float m_enemyVoice = 5.0f;
    // Start is called before the first frame update
    public virtual void Start()
    {
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        //元々のスピードを保存。
        m_defaultSpeed = m_agent.speed;

        // AudioSource配列を初期化
        m_audioSource = new AudioSource[m_soundClip.Length];

        for (int i = 0; i < m_soundClip.Length; i++)
        {
            m_audioSource[i] = gameObject.AddComponent<AudioSource>();
            m_audioSource[i].clip = m_soundClip[i];
            m_audioSource[i].playOnAwake = false; // 自動再生しない

            m_audioSource[i].spatialBlend = 1.0f;
            m_audioSource[i].rolloffMode = AudioRolloffMode.Logarithmic;
            m_audioSource[i].minDistance = 1.0f;
            m_audioSource[i].maxDistance = 20.0f;

        }

        m_targetPlayer = GameObject.FindWithTag("Player");
        m_navPoint = gameObject.AddComponent<NavPointList>();

        m_agent.updateRotation = true;

        // Rigidbody を無効化（NavMeshAgent に任せる）
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (DebugStop == true)
            return;

        if (!m_agent.pathPending) // 経路計算が完了していて
        {
            if (m_agent.remainingDistance <= m_agent.stoppingDistance) // 残り距離が停止距離以下
            {
                if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude <= 3f) // 経路がなく、停止している
                {
                    m_enemyState = EnemyState.enEnemyState_Lost;
                }
            }
        }
    }


    //-------------------------------------------------------------------------------//
    //汎用処理。
    //基本的にいじらないでください、不備があったら河田まで。

    //プレイヤーを見つける。
    protected bool PlayerSearch(float rayRange)
    {
        if (m_targetPlayer == null) return false;

        // 敵の目の高さからレイを飛ばす
        Vector3 startPos = transform.position + Vector3.up * 1.5f;
        Vector3 diff = m_targetPlayer.transform.position - startPos;
        Vector3 dir = diff.normalized;

        // レイを描画
        Debug.DrawRay(startPos, diff.normalized * rayRange, Color.red, 0.1f);

        // レイヤーマスク (例: Default と Player のみ)
        int layerMask = LayerMask.GetMask("Default", "Player");

        // レイを発射
        RaycastHit hit;
        if (Physics.Raycast(startPos, dir, out hit, rayRange, layerMask))
        {
            // 視野角チェック
            if (Vector3.Angle(transform.forward, dir) <= m_searchAngle)
            {
                // 最初にヒットしたのがプレイヤーなら「見えている」
                if (hit.collider.CompareTag("Player"))
                {
                    m_NextMovePos = m_targetPlayer.transform.position;
                    return true;
                }
            }
        }
        return false;
    }

    //ダメージを受ける。また、ダメージのレベルによって処理を変更できる。(死亡判定もここで行う)
    //ノックバックアニメーションが修正不可能なため、damageLevelに0以外の数字を入れないでください。
    public void TakeDamage(float damage, int damageLevel)
    {
        AttackColliderFalse();
        m_hp -= damage;

        if (damageLevel == 0)
        {
            m_enemyState = EnemyState.enEnemyState_Damage;
        }
        else if (damageLevel == 1)
        {
            //m_enemyState = EnemyState.enEnemyState_Stun;
        }

        if (m_hp <= 0)
        {
            m_enemyState = EnemyState.enEnemyState_Death;
            DebugStop = true;
        }
    }

    //移動処理。
    protected void Move(float speedMultiplier = 1f)
    {
        m_agent.speed = m_defaultSpeed * speedMultiplier;

        Vector3 direction = m_NextMovePos - transform.position;

        if (direction.sqrMagnitude > 1.0f)
        {
            m_agent.SetDestination(m_NextMovePos);

            // ★ 到達不可能チェック
            if (!CheckPathReachable())
            {
                m_enemyState = EnemyState.enEnemyState_Lost;
                m_agent.isStopped = true;
                return;
            }

            m_agent.isStopped = false;
        }
        else
        {
            m_agent.isStopped = true;
        }
    }


    //ナビメッシュが目標地点へ到達できるかチェック。
    protected bool CheckPathReachable()
    {
        if (m_agent.pathStatus == NavMeshPathStatus.PathInvalid ||
            m_agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            // 到達不可能
            return false;
        }
        return true;
    }

    //スタック対策。
    protected void CheckStuck()
    {
        float moved = Vector3.Distance(transform.position, m_lastPos);

        if (moved < stuckThreshold)
        {
            m_stuckTimer += Time.deltaTime;

            if (m_stuckTimer >= stuckTimeRequired)
            {
                m_navActive = true;
                m_stateLook = false;
                m_enemyState = EnemyState.enEnemyState_Search;
                m_stuckTimer = 0f;
            }
        }
        else
        {
            m_stuckTimer = 0f;
        }

        m_lastPos = transform.position;
    }

    //次の行き先を決定する。(m_navActiveがtrueの場合のみ実行)
    protected void SetNavMovePos()
    {
        if (m_navPoint.GetPointNum() == 0) return;

        int nextTarget;
        do
        {
            nextTarget = Random.Range(0, m_navPoint.GetPointNum());
        } while (nextTarget == m_currentTarget); // 同じ場所を避ける

        m_currentTarget = nextTarget;
        m_NextMovePos = m_navPoint.GetPointPos(nextTarget);

        m_enemyState = EnemyState.enEnemyState_Search;
        m_navActive = false;
    }

    //サウンドの再生。
    public void PlaySound(int i) =>
        m_audioSource[i].Play();

    //ダメージによる移動処理の停止。
    protected void DamageAnimation()
    {
        m_stateLook = true;

        // ★ NavMeshAgent を止める（EnemyBase に m_navAgent がある前提）
        if (m_agent != null)
        {
            m_agent.isStopped = true;
            m_agent.updatePosition = false;
            m_agent.updateRotation = false;
            m_agent.velocity = Vector3.zero;
        }

        m_animator.applyRootMotion = true;

        // ★ ダメージアニメーションが終わったら復帰
        if (AnimationEndCheck("Damage") || AnimationEndCheck("Knockback")
            || AnimationEndCheck("damage") || AnimationEndCheck("Hit"))
        {
            m_stateLook = false;

            m_animator.applyRootMotion = false;

            // NavMeshAgent を再開
            if (m_agent != null)
            {
                m_agent.isStopped = false;
                m_agent.updatePosition = true;
                m_agent.updateRotation = true;
            }

            m_enemyState = EnemyState.enEnemyState_Stun;
            m_stateLook = true;
            m_Stan = true;
        }
    }

    //スタンするとIdleステートで待機状態にする。
    protected void StunTimer(float time)
    {
        m_stunTimer += Time.deltaTime;
        if (m_stunTimer > time)
        {
            m_stunTimer = 0;
            m_Stan = false;
            m_stateLook = false;
            m_enemyState = EnemyState.enEnemyState_Search;
            SetNavMovePos();
        }
    }

    //アタック時のダメージ判定用コリジョンを使用可能、使用不可にする。
    protected void AttackColliderTrue() =>
        m_attackCollider.SwitchWnabled(true);
    protected void AttackColliderFalse() =>
        m_attackCollider.SwitchWnabled(false);

    //ここからは判定、リセット系。
    //クールタイム計算。
    protected bool GetCooldown(float time)
    {
        m_attackCooldown += Time.deltaTime;
        if (m_attackCooldown >= time)
        {
            m_attackCooldown = 0f;
            return true;
        }
        return false;
    }
    //宣言でアニメーション内のすべての変数のリセット。
    public void ResetAllAnimatorParameters()
    {
        foreach (AnimatorControllerParameter param in m_animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    m_animator.SetBool(param.name, false);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    m_animator.ResetTrigger(param.name);
                    break;
                case AnimatorControllerParameterType.Float:
                    m_animator.SetFloat(param.name, 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    m_animator.SetInteger(param.name, 0);
                    break;
            }
        }
    }

    bool wasAttack = false;
    //アニメーションが始まったかを判定。
    public bool AnimationStartCheck(string animeName)
    {
        var state = m_animator.GetCurrentAnimatorStateInfo(0);
        bool isAttack = state.IsName(animeName);

        bool started = isAttack && !wasAttack; wasAttack = isAttack;
        return started;
    }

    //アニメーションが終わったかを判定。
    public bool AnimationEndCheck(string animeName)
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animeName) && stateInfo.normalizedTime >= 1f)
        {
            return true;
        }
        return false;
    }

    float lostTimer = 0.0f;
    //Lostステートに入ってから指定秒数後次のステートへ移行できるようにする。
    protected void LostKeepTime(float lostTime)
    {
        lostTimer += Time.deltaTime;

        if (lostTimer >= lostTime)
        {
            lostTimer = 0.0f;
            m_navActive = true;
        }
    }

    protected bool SoundTimer(float soundTime)
    {
        soundTimer += Time.deltaTime;

        if (soundTimer >= soundTime)
        {
            soundTimer = 0.0f;
            return true;
        }
        return false;
    }

    //ランダムにボイスタイマーをセットする。
    protected void SetRandamTimer() =>
        m_enemyVoice = Random.Range(4.0f, 15.0f);
    //-------------------------------------------------------------------------------//

    //固有処理。
    public virtual void UpdateState() { }
    public virtual void StartAttack() { }
    public virtual void EndAttack() { }
}
