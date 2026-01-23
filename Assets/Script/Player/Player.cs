using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Avoid,
        Attack,
        Dead,
    }


    //パラメータ
    [SerializeField] GameObject m_BarObject;
    UI_Gauge m_uIGauge;
    [SerializeField] private float m_maxHPGauge = 100.0f;
    private float m_hpGauge = 100.0f;

    public float GetMaxHP()
    {
        return m_maxHPGauge;
    }
    public float GetPlayerHP()
    {
        return m_hpGauge;
    }

    [SerializeField] private float m_maxStaminaGauge = 100.0f;
    private float m_staminaGauge = 100.0f;
    public float GetMaxStamina()
    {
        return m_maxStaminaGauge;
    }
    public float GetStamina()
    {
        return m_staminaGauge;
    }

    //スタミナ
    private float m_runStamina = 20.0f;
    private float m_moveStaminaRecovery = 20.0f;
    private bool m_staminaRecoveryFlag = true;
    //移動速度
    [SerializeField] private float m_moveSpeed = 5.0f;
    private float m_walkSpeed = 5.0f;
    private float m_runSpeed = 10.0f;
    //ジャンプ力
    public float m_jumpForce = 5.0f;
    //足音タイミングの変数
    private float t = 0.5f;
    //地面の接触判定
    private bool m_isGround = true;
    //プレイヤーステート
    private PlayerState m_playerState = PlayerState.Idle;
    //移動スティックの入力
    private Vector3 stickL = Vector3.zero;

    //回避アクション用変数
    PlayerAvoid m_playerAvoid;

    //ゲームオーバー変数
    GameOver m_gameOver;

    //獲得したアイテムの総重量。
    [SerializeField] private float m_totalWeight = 0.0f;
    //重さによる倍率。
    [SerializeField] float wightRatio = 0.0f;
    //重さの基準値。
    float baseWight = 60.0f;

    //アニメーション
    [SerializeField] Animator m_animator;
    [SerializeField] GameObject m_playerAnimObject;
    [SerializeField] GameObject m_slashAnimObject;

    //キャッシュ
    Rigidbody m_rigidBody;

    //効果音
    public float m_stepSEVolume = 1.0f;
    public float m_stepSEPitch = 1.0f;
    float m_stepSEInterval = 0.5f;
    float m_stepSEWalkInterval = 0.5f;
    float m_stepSERunInterval = 0.35f;
    [SerializeField]
    AudioClip m_attackSE, m_stepSE, m_jumpSE, m_recoverySE, m_damageSE;
    

    //ステート変更用関数
    public void SetPlayerState(PlayerState state)
    {
        m_playerState = state;
    }

    // Start is called before the first frame update
    void Awake()
    {
        //必要な情報を取得
        m_playerAvoid = GetComponent<PlayerAvoid>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = m_playerAnimObject.GetComponent<Animator>();
        m_slashAnimObject.SetActive(false);
        m_hpGauge = m_maxHPGauge;
        m_staminaGauge = m_maxStaminaGauge;
        m_uIGauge = m_BarObject.GetComponent<UI_Gauge>();
        //m_uIGauge.UpdateStaminaGauge();
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
            case PlayerState.Avoid:
                Avoid();
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.Dead:
                Dead();
                break;
        }
    }

    void Idle()
    {
        if (m_playerState == PlayerState.Dead)
        {
            return;
        }

        m_animator.SetBool("Idle", true);
        m_animator.SetBool("Walk", false);
        m_animator.SetBool("Run", false);
        stickL = Vector3.zero;
        //スタミナが減っていたら回復する
        if (m_staminaGauge < 100.0f)
        {
            StaminaWeightModifier(m_totalWeight, baseWight);
            RecoveryStamina(m_moveStaminaRecovery * 1.5f / wightRatio);
        }

        //他のステートに移行する
        if (Input.GetAxis("Run") > 0.1f || Input.GetButton("Jump"))
        {
            m_playerState = PlayerState.Move;
        }
        else if (Input.GetAxis("Horizontal") > 0.1f || Input.GetAxis("Horizontal") < -0.1f ||
            Input.GetAxis("Vertical") > 0.1f || Input.GetAxis("Vertical") < -0.1f)
        {
            m_playerState = PlayerState.Move;
        }
        else if (Input.GetAxis("Horizontal_Pad") > 0.1f || Input.GetAxis("Horizontal_Pad") < -0.1f ||
            Input.GetAxis("Vertical_Pad") > 0.1f || Input.GetAxis("Vertical_Pad") < -0.1f)
        {
            m_playerState = PlayerState.Move;
        }
    }

    void Move()
    {
        if (m_playerState == PlayerState.Dead)
        {
            return;
        }

        //カメラを考慮した移動
        Vector3 PlayerMove = Vector3.zero;
        stickL = Vector3.zero;

        if(Input.GetAxis("Vertical")!=0.0f || Input.GetAxis("Horizontal") != 0.0f)
        {
            stickL.z = Input.GetAxis("Vertical");
            stickL.x = Input.GetAxis("Horizontal");
        }
        else if (Input.GetAxis("Vertical_Pad") != 0.0f || Input.GetAxis("Horizontal_Pad") != 0.0f)
        {
            stickL.z = Input.GetAxis("Vertical_Pad");
            stickL.x = Input.GetAxis("Horizontal_Pad");
        }

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

        //他のステートに移行
        //Runキーが押されている場合
        if (Input.GetAxis("Run") > 0.1f && m_staminaGauge > 0.0f && stickL.magnitude > 0.1f)
        {
            m_animator.SetBool("Run", true);
            m_animator.SetBool("Walk",false);
            m_animator.SetBool("Idle", false);
            ////重量によってスタミナの増幅幅を変更。
            StaminaWeightModifier(m_totalWeight, baseWight);
            UseStamina(m_runStamina, wightRatio);
            m_moveSpeed = m_runSpeed;

            //足音のピッチ変更
            m_stepSEPitch = 2.0f;
            m_stepSEInterval = m_stepSERunInterval;
        }
        //Avoid(回避)キーが押されている場合
        else if (Input.GetButtonDown("Avoid") && m_staminaGauge > 10.0f && stickL.magnitude > 0.1f)
        {
            m_playerState = PlayerState.Avoid;
        }
        //移動キーのみの場合(歩き)
        else
        {
            m_animator.SetBool("Walk", true);
            m_animator.SetBool("Run", false);
            m_animator.SetBool("Idle", false);
            m_moveSpeed = m_walkSpeed;

            //重量によってスタミナの増幅幅を変更。
            StaminaWeightModifier(m_totalWeight, baseWight);
            RecoveryStamina(m_moveStaminaRecovery / wightRatio);

            //足音のピッチ変更
            m_stepSEPitch = 1.0f;
            m_stepSEInterval = m_stepSEWalkInterval;
        }

        //スペースが押されたらジャンプ
        if (m_isGround == true && m_staminaGauge > 0.0f)
        {
            if (Input.GetButton("Jump"))
            {
                m_animator.SetBool("Walk", false);
                m_animator.SetBool("Run", false);
                m_animator.SetTrigger("Jump");

                UseStamina(100.0f, 1.0f);
                m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
                m_isGround = false;
                m_stepSEPitch = 1.5f;
                GameManager.PlaySE(m_jumpSE,m_stepSEVolume, m_stepSEPitch);
            }
        }

        //回避ボタンが押されたら回避
        if (Input.GetAxis("Avoid") > 0.1f)
        {
            m_playerState = PlayerState.Avoid;
        }

        //プレイヤーの速度を設定することで移動させる
        PlayerMove = (PlayerMove * m_moveSpeed);
        PlayerMove.y = m_rigidBody.velocity.y;
        m_rigidBody.velocity = PlayerMove;
        
        if (stickL != Vector3.zero && m_isGround == true)
        {
            //足音用
            t += Time.deltaTime;
            if (t > m_stepSEInterval)
            {
                GameManager.PlaySE(m_stepSE,m_stepSEVolume,m_stepSEPitch);
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
            m_isGround = true;
        }
    }

    //スタミナ使用関数
    void UseStamina(float stamina, float ratio)
    {
        m_staminaGauge -= stamina * Time.deltaTime * ratio;
        m_uIGauge.UpdateStaminaGauge();
    }

    //スタミナ回復用関数
    async void RecoveryStamina(float stamina)
    {
        if(m_staminaGauge >= 100.0f)
        {
            return;
        }

        if(m_staminaGauge < 0.0f)
            m_staminaGauge = 0.0f;

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
        m_uIGauge.UpdateStaminaGauge();
    }

    //スタミナの増減幅をプレイヤーの重量によって変更する値を決定。
    void StaminaWeightModifier(float weight, float baseWight)
    {
        //wightRatio = (weight == 0.0f ? 1.0f : Mathf.Pow(weight, 1.0f / root));
        wightRatio = 1.0f;

        if (weight == 0.0f){
            return;
        }


        float ratio = weight / baseWight;
        wightRatio += ratio;
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

    //プレイヤーが回避する
    void Avoid()
    {
        m_playerAvoid.Avoid();
        m_playerState = PlayerState.Idle;
    }

    //プレイヤーがアタックする
    public void Attack()
    {
        m_playerState = PlayerState.Attack;
        m_animator.SetTrigger("Attack");
        m_slashAnimObject.SetActive(true);
        GameManager.PlaySE(m_attackSE);
    }

    //プレイヤーが回復する処理
    public void RecoveryHP(float hp)
    {
        m_hpGauge += hp;
        GameManager.PlaySE(m_recoverySE);
        if (m_hpGauge > 100.0f)
        {
            m_hpGauge = 100.0f;
        }
        m_uIGauge.UpdateHPGauge();
    }

    //プレイヤーがダメージを受けた時
    public void TakeDamage(float damage)
    {
        m_hpGauge -= damage;
        GameManager.PlaySE(m_damageSE);
        if(m_hpGauge <= 0.0f)
        {
            m_hpGauge = 0.0f;
            m_playerState = PlayerState.Dead;
        }
        m_uIGauge.UpdateHPGauge();
    }

    //プレイヤーが4んだ時
    public void Dead()
    {
        m_gameOver = GameObject.FindWithTag("GameOver").GetComponent<GameOver>();
        m_gameOver.SetGameOver();
        if(m_animator.GetBool("Dead") == false)
        {
            m_animator.SetBool("Dead", true);
        }
    }

    void Update()
    {
        //プレイ中でないなら中断
        if (GameManager.GetGameState() != GameManager.GameState.enGameState_Play)
        {
            return;
        }

        PlayerStatus();
    }
}
