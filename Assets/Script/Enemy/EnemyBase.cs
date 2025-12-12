using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected AttackCollider m_attackCollider;
    [SerializeField] protected Animator m_animator;

    protected GameObject m_targetPlayer;

    [SerializeField] protected AudioClip[] m_soundClip;
    private AudioSource[] m_audioSource;

    protected NavMeshAgent m_agent;
    protected Rigidbody rb;

    [SerializeField] protected float m_searchAngle;

    NavPointList m_navPoint;
    int m_currentTarget = -1;
    protected bool m_navActive = false;

    [SerializeField]  protected Vector3 m_NextMovePos = Vector3.zero;             //次の移動先。

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

    [SerializeField]  protected EnemyState m_enemyState = EnemyState.enEnemyState_Search;
    [SerializeField]  public bool m_stateLook = false;

    [SerializeField] float m_hp;

    float m_cooldown = 100.0f;
    [SerializeField] protected float m_attackCoolTime;

    //デバック用変数。
    //死亡時に全ての処理を停止させる
    protected bool DebugStop = false;

    private float soundTimer = 100.0f;

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // AudioSource配列を初期化
        m_audioSource = new AudioSource[m_soundClip.Length];

        for (int i = 0; i < m_soundClip.Length; i++)
        {
            m_audioSource[i] = gameObject.AddComponent<AudioSource>();
            m_audioSource[i].clip = m_soundClip[i];
            m_audioSource[i].playOnAwake = false; // 自動再生しない
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
        {
            return;
        }

        soundTimer += Time.deltaTime;

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

    //汎用処理。
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
    public void TakeDamage(float damage, int damageLevel)
    {
        m_hp -= damage;
        if (damageLevel == 0)
        {
            m_enemyState = EnemyState.enEnemyState_Damage;
        }
        else if (damageLevel == 1)
        {
            m_enemyState = EnemyState.enEnemyState_Stun;
        }

        if (m_hp <= 0)
        {
            m_enemyState = EnemyState.enEnemyState_Death;
            DebugStop = true;
        }
    }
    //移動処理。
    protected void Move() 
    {
        Vector3 direction = m_NextMovePos - transform.position;
        float distance = direction.magnitude;

        if (direction.sqrMagnitude > 1.0f)
        {
            m_agent.SetDestination(m_NextMovePos);
            m_agent.isStopped = false;
        }
        else
        {
            m_agent.isStopped = true;
        }
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

    public void PlaySound(int i) =>
        m_audioSource[i].Play();

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

    //クールタイム計算。
    protected bool GetCooldown(float time)
    {
        m_cooldown += Time.deltaTime;
        if (m_cooldown >= time){
            m_cooldown = 0f;
            return true;
        }
        return false;
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
        if (soundTimer >= soundTime)
        {
            soundTimer = 0.0f;
            return true;
        }
        return false;
    }

    //固有処理。
    public virtual void UpdateState() { }

    public virtual void StartAttack() { }

    public virtual void EndAttack() { }
}
