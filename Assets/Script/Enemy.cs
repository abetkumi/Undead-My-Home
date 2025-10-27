using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject m_targetPlayer;
    [SerializeField] AudioClip StepSE;

    NavMeshAgent m_agent;
    Animator m_animator;
    private Rigidbody rb;

    Transform[] m_navPoints = new Transform[9];
    int m_currentTarget = -1;
    [SerializeField] private bool m_navActive = false;

    [SerializeField] private Vector3 m_NextMovePos = Vector3.zero;             //次の移動先。

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
    //int m_targetNum = 0;
    //bool m_targetMode = false;

    [SerializeField] float m_hp;
    [SerializeField] float m_speed,m_dashSpeed;

    const float CHASE_RANGE = 120.0f;
    const float ATTACK_RANGE = 30.0f;

    [SerializeField]
    float m_searchAngle, m_searchRayRange, m_chaseRayRange;

    //デバック用変数。
    //死亡時に全ての処理を停止させる
    bool DebugStop = false;

    void TargetAdd(int add)
    {

    }

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        m_agent.updateRotation = true;

        // Rigidbody を無効化（NavMeshAgent に任せる）
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        SetNavMeshPos();
        m_animator.SetBool("Move", true);
    }

    Vector3 lastTargetPos;
    float updateThreshold = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (DebugStop == true){
            return;
        }

        Vector3 playerPos = m_targetPlayer.transform.position;
        if (PlayerSearch(m_searchRayRange))
        {
            float sqrDist = (playerPos - lastTargetPos).sqrMagnitude;
            if (sqrDist > updateThreshold * updateThreshold)
            {
                m_NextMovePos = playerPos;
                lastTargetPos = playerPos;
            }

            if ((transform.position - playerPos).sqrMagnitude <= ATTACK_RANGE)
            {
                m_enemyState = EnemyState.enEnemyState_Attack;
            }
            else
            {
                m_enemyState = EnemyState.enEnemyState_Chase;
            }
        }
        else
        {
            //m_enemyState = EnemyState.enEnemyState_Lost;
        }

        if (Input.GetButton("testKye1")){
            m_navActive = true;
        }
        else if (Input.GetButton("Jump")){
            TakeDamage(10.0f, 0);
        }

        if (m_navActive) { SetNavMovePos(); }

        switch (m_enemyState){
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
                Move();
                break;
            //見失う。
            case EnemyState.enEnemyState_Lost:
                m_animator.SetTrigger("Lost");
                m_animator.SetBool("Chaes", false);
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
                m_animator.SetBool("Move", false);
                m_animator.SetTrigger("Death");
                break;
            //それ以外。
            default:
                break;
        }
    }

    //ナビメッシュ用のポイントの座標を登録。(一回のみ実行)
    void SetNavMeshPos()
    {
        for(int i = 0; i < 9; i++)
        {
            string pointName = "Point" + (i + 1).ToString("D3");
            GameObject pointObj = GameObject.Find(pointName);
            if (pointObj != null){
                m_navPoints[i] = pointObj.transform;
            }
            else{
                Debug.LogWarning($"{pointName} が見つかりませんでした");
            }
        }
    }

    // プレイヤーを探す 見つけたらtrueを返す
    bool PlayerSearch(float rayRange)
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

    void StartAttack()
    {
        m_attackCollider.SwitchWnabled(true);
        Invoke("EndAttack", 3.0f);          //3秒後に判定を消す。
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

    //ナビメッシュ用の移動処理。なんかグルグルしてるから修正待ってね。
    //void Move(float inSpeed)
    //{
    //    m_agent.speed = inSpeed;

    //    Vector3 direction = m_NextMovePos - transform.position;
    //    float distance = direction.magnitude;

    //    // Raycastで障害物があるかチェック
    //    bool hasObstacle = Physics.Raycast(transform.position, direction.normalized, distance);

    //    if (!hasObstacle)
    //    {
    //        // 障害物がない → Rigidbodyで直進
    //        rb.MovePosition(transform.position + direction.normalized * m_agent.speed * Time.deltaTime);
    //        transform.LookAt(new Vector3(m_NextMovePos.x, transform.position.y, m_NextMovePos.z));
    //    }
    //    else
    //    {
    //        // 障害物がある → NavMeshAgentで経路移動
    //        if (direction.sqrMagnitude > 1.0f)
    //        {
    //            m_agent.SetDestination(m_NextMovePos);
    //            m_agent.isStopped = false;
    //        }
    //        else
    //        {
    //            m_agent.isStopped = true;
    //        }
    //    }
    //}

    void Move()
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
    void SetNavMovePos()
    {
        if (m_navPoints.Length == 0) return;

        int nextTarget;
        do
        {
            nextTarget = Random.Range(0, m_navPoints.Length);
        } while (nextTarget == m_currentTarget); // 同じ場所を避ける

        m_currentTarget = nextTarget;
        m_NextMovePos = m_navPoints[nextTarget].position;

        m_enemyState = EnemyState.enEnemyState_Search;
        m_navActive = false;
    }
}