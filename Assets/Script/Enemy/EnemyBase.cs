using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected GameObject m_targetPlayer;

    NavMeshAgent m_agent;
    protected Rigidbody rb;

    [SerializeField] protected float m_searchAngle;

    [SerializeField] NavPointList m_navPoint;
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
        enEnemyState_Num,       //ステートの数。
    }

    [SerializeField]  protected EnemyState m_enemyState = EnemyState.enEnemyState_Search;
    public bool m_stateLook = false;

    [SerializeField] float m_hp;

    //デバック用変数。
    //死亡時に全ての処理を停止させる
    protected bool DebugStop = false;

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

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
    }

    //汎用処理。
    //プレイヤーを見つける。
    protected bool PlayerSearch(float rayRange)
    {
        // レイの始点を計算
        Vector3 startPos = transform.position;
        startPos.y += 10.0f;
        // プレイヤーへ伸びるベクトルを計算
        Vector3 diff = m_targetPlayer.transform.position - startPos;

        // レイを描画
        Debug.DrawRay(startPos, diff.normalized * rayRange, Color.red, 0.1f);

        // レイを発射
        RaycastHit hit;
        if (Physics.Raycast(startPos, diff.normalized, out hit, rayRange))
        {
            // プレイヤーが視野角内かつレイが最初にヒットしたのがプレイヤーだったら…
            if (Vector3.Angle(transform.forward, diff) <= m_searchAngle
                && hit.collider.CompareTag("Player"))
            {
                m_NextMovePos = m_targetPlayer.transform.position;
                // プレイヤー発見
                return true;
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


    //固有処理。
    public virtual void UpdateState() { }

    public virtual void StartAttack() { }

    public virtual void EndAttack() { }
}
