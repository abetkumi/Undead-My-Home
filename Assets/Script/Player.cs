using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{

    private enum PlayerState
    {
        Idle,
        Move,
        Dead,
    }

   
    //パラメータ
    [SerializeField] private float m_hpGauge = 100.0f;
    [SerializeField] private float m_staminaGauge = 100.0f;
    private float m_runStamina = 20.0f;
    private float m_idleStaminaRecovery = 7.0f;
    private float m_moveStaminaRecovery = 20.0f;
    private bool m_staminaRecoveryFlag = true;
    [SerializeField] private float m_moveSpeed = 500.0f;
    private float m_walkSpeed = 500.0f;
    private float m_runSpeed = 1000.0f;
    private float t = 0.5f;
    private bool isGround = true;
    public float jumpForce = 50.0f;
    private PlayerState m_playerState = PlayerState.Idle;
    private Vector3 stickL = Vector3.zero;

    //獲得したアイテムの総重量。
    [SerializeField] private float m_totalWeight = 0.0f;
    //重さによる倍率。
    [SerializeField] float wightRatio = 0.0f;
    //乗数。
    float n = 10.0f;

    //アニメーション
    Animator m_animator;

    //キャッシュ
    Rigidbody m_rigidBody;

    public void GetHPDamage(float damage)
    {
        m_hpGauge -= damage;
        if(m_hpGauge <= 0)
        {
            m_playerState = PlayerState.Dead;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //必要な情報を取得
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    void PlayerStatus()
    {
        switch (m_playerState)
        {
            case PlayerState.Idle:
                Idle();
                break;
            case PlayerState.Move:
                Move();
                break;
            case PlayerState.Dead:
                Dead();
                break;
        }
    }

    void Idle()
    {
        stickL = Vector3.zero;
        //スタミナが減っていたら回復する
        if (m_staminaGauge < 100.0f)
        {
            RecoveryStamina(m_idleStaminaRecovery);
        }

        if (Input.anyKey)
        {
            m_playerState = PlayerState.Move;
        }
    }

    void Move()
    {
        //カメラを考慮した移動
        Vector3 PlayerMove = Vector3.zero;
        stickL = Vector3.zero;
        stickL.z = Input.GetAxis("Vertical");
        stickL.x = Input.GetAxis("Horizontal");
        if (stickL.magnitude <= 0.1f)
        {
            m_playerState = PlayerState.Idle;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;

        right *= stickL.x;
        forward *= stickL.z;

        //移動速度に上記で計算したベクトルを加算する
        PlayerMove += right + forward;

 

        //Runキーが押されている場合
        if (Input.GetButton("Run") && m_staminaGauge > 0.0f && stickL.magnitude > 0.1f)
        {
            ////重量によってスタミナの増幅幅を変更。
            StaminaWeightModifier(m_totalWeight, n);
            UseStamina(m_runStamina, wightRatio);
            m_moveSpeed = m_runSpeed;
        }
        else
        {
            m_moveSpeed = m_walkSpeed;

            //重量によってスタミナの増幅幅を変更。
            StaminaWeightModifier(m_totalWeight, n);
            RecoveryStamina(m_moveStaminaRecovery * (1 / wightRatio));
        }

        //スペースが押されたらジャンプ
        if (isGround == true && m_staminaGauge > 0.0f)
        {
            if (Input.GetButton("Jump"))
            {
                UseStamina(10.0f, 1.0f);
                m_rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGround = false;
            }
        }
        //プレイヤーの速度を設定することで移動させる
        PlayerMove = (PlayerMove * m_moveSpeed * Time.deltaTime);
        PlayerMove.y = m_rigidBody.velocity.y;
        m_rigidBody.velocity = PlayerMove;

 
        
        if (stickL != Vector3.zero)
        {
            //足音用
            t += Time.deltaTime;
            if (t > 0.5f)
            {
                t = 0.0f;
            }
        }
        else if (stickL == Vector3.zero)
        {
            t = 0.0f;
            m_playerState = PlayerState.Idle;
        }
    }

    //地面の接触判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    //スタミナ使用関数
    void UseStamina(float stamina, float ratio)
    {
        m_staminaGauge -= stamina * Time.deltaTime * ratio;
    }

    //スタミナ回復用関数
    async void RecoveryStamina(float stamina)
    {
        //スタミナが0になると回復開始を遅らせる
        if (m_staminaGauge <= 0)
        {
            m_staminaRecoveryFlag = false;
            //走っていない場合回復を開始する
            if (stickL.magnitude < 0.1f || !Input.GetButton("Run"))
            {
                await UniTask.Delay(1000);
                m_staminaRecoveryFlag = true;
            }
        }

        if (m_staminaRecoveryFlag == false)
        {
            return;
        }

        //スタミナが減っていたら回復する
        if (m_staminaGauge < 100.0f)
        {
            m_staminaGauge += stamina * Time.deltaTime;
            //スタミナが上限を超えていたら上限に戻す
            if (m_staminaGauge > 100.0f)
            {
                m_staminaGauge = 100.0f;
            }
        }
    }

    //スタミナの増減幅をプレイヤーの重量によって変更する値を決定。
    void StaminaWeightModifier(float weight, float root)
    {
        wightRatio = (weight == 0.0f ? 1.0f : Mathf.Pow(weight, 1.0f / root));
    }

    //プレイヤーがアイテムを取得した時の重さの加算と減算。
    //引数がtrueの時は取得、falseの時は捨てる。
    public void ItemWeightAdd(float weight, bool get)
    {
        if (get == true){
            m_totalWeight += weight;
        } else {
            m_totalWeight -= weight;
        }
    }

    void Dead()
    {
        m_animator.SetBool("Dead", true);
    }

    void FixedUpdate()
    {
        //プレイ中でないなら中断
        if (GameManager.GetGameState() != GameManager.GameState.enGameState_Play)
        {
            return;
        }

        PlayerStatus();
    }
}
