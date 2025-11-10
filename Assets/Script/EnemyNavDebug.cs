using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;



public class EnemyNavDebug : MonoBehaviour
{
    NavMeshAgent m_agent;
    private Rigidbody rb;

    Transform[] m_navPoints = new Transform[9];
    int m_currentTarget = -1;
    [SerializeField] private bool m_navActive = false;

    [SerializeField] private Vector3 m_NextMovePos = Vector3.zero;



    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        //rb = GetComponent<Rigidbody>();

        m_agent.updateRotation = true;

        // Rigidbody を無効化（NavMeshAgent に任せる）
        //if (rb != null)
        //{
        //    rb.isKinematic = true;
        //}

        SetNavMeshPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("testKye1"))
        {
            SetNavMovePos();
        }

        Move();
    }

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

    //ナビメッシュ用のポイントの座標を登録。(一回のみ実行)
    void SetNavMeshPos()
    {
        for (int i = 0; i < 9; i++)
        {
            string pointName = "Point" + (i + 1).ToString("D3");
            GameObject pointObj = GameObject.Find(pointName);
            if (pointObj != null)
            {
                m_navPoints[i] = pointObj.transform;
            }
            else
            {
                Debug.LogWarning($"{pointName} が見つかりませんでした");
            }
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

        m_navActive = false;
    }
}
