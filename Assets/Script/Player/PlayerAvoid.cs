using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー回避アクション用スクリプト
public class PlayerAvoid : MonoBehaviour
{
    [SerializeField] GameManager m_gameManager;
    [Header("回避設定")]
    public float m_avoidDistance = 5.0f;    // 回避距離
    public float m_avoidDuration = 0.2f;  // 回避時間
    public int m_avoidCooldown = 3;  // クールタイム（回避後の待ち時間）

    [Header("無敵設定")]
    public int m_invincibleTime = 1; // 無敵時間（回避開始からの秒数）

    private Rigidbody rb;
    private bool m_isAvoiding = false;
    private bool m_canAvoid = true;
    private bool m_isInvincible = false;

    private Vector3 m_avoidStartPos;
    private Vector3 m_avoidEndPos;
    private float m_avoidTimer = 0f;

    private CapsuleCollider m_playerCollider; // 無敵時に当たり判定をOFFにする場合用

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_playerCollider = GetComponent<CapsuleCollider>();
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        //UIに回避可能時表示
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_RB,
                "回避", true);
    }

    public void Avoid()
    {
        // 回避入力受付
        if (!m_canAvoid)
        {
            return;
        }

        if(m_isAvoiding)
        {
            return;
        }

        int m_direction = 0;

        // ゲームパッド操作（左スティック）
        float horizontal = Input.GetAxis("Horizontal"); // -1:左, +1:右

        if (horizontal > 0.5f)
        {
            m_direction = 1;
        }
        else if (horizontal < -0.5f) 
        {
            m_direction = -1;
        }

        // 回避開始
        if (m_direction != 0)
        {
            Avoid(m_direction);
        }
        
    }

    private async void Avoid(int direction)
    {
        m_canAvoid = false;
        m_isAvoiding = true;

        m_avoidStartPos = rb.position;
        m_avoidEndPos = rb.position + transform.right * direction * m_avoidDistance;
        m_avoidTimer = 0f;

        //UIに回避可能時表示
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_RB,
                "回避", false);

        // 無敵開始
        Invincible();

        // 回避モーション
        while (m_avoidTimer < m_avoidDuration)
        {
            m_avoidTimer += Time.fixedDeltaTime;
            float t = m_avoidTimer / m_avoidDuration;
            Vector3 newPos = Vector3.Lerp(m_avoidStartPos, m_avoidEndPos, t);
            rb.MovePosition(newPos);

            // FixedUpdateタイミングで次を待つ
            await UniTask.WaitForFixedUpdate();
        }

        m_isAvoiding = false;
        Debug.Log("回避!");
        // クールタイム待機
        await UniTask.Delay(TimeSpan.FromSeconds(m_avoidCooldown));
        m_canAvoid = true;

        //UIに回避可能時表示
        m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_RB,
                "回避", true);

        Debug.Log("回避クールタイム終了");
    }

    private async void Invincible()
    {
        m_isInvincible = true;

        await UniTask.DelayFrame(m_invincibleTime);

        m_isInvincible = false;
    }

    // 外部スクリプトから無敵判定を参照できるようにする
    public bool IsInvincible()
    {
        return m_isInvincible;
    }
}
