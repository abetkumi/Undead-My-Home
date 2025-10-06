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

   
    //�p�����[�^
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

    //�l�������A�C�e���̑��d�ʁB
    [SerializeField] private float m_totalWeight = 0.0f;
    //�d���ɂ��{���B
    [SerializeField] float wightRatio = 0.0f;
    //�搔�B
    float n = 10.0f;

    //�A�j���[�V����
    Animator m_animator;

    //�L���b�V��
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
        //�K�v�ȏ����擾
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
        //�X�^�~�i�������Ă�����񕜂���
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
        //�J�������l�������ړ�
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

        //�ړ����x�ɏ�L�Ōv�Z�����x�N�g�������Z����
        PlayerMove += right + forward;

 

        //Run�L�[��������Ă���ꍇ
        if (Input.GetButton("Run") && m_staminaGauge > 0.0f && stickL.magnitude > 0.1f)
        {
            ////�d�ʂɂ���ăX�^�~�i�̑�������ύX�B
            StaminaWeightModifier(m_totalWeight, n);
            UseStamina(m_runStamina, wightRatio);
            m_moveSpeed = m_runSpeed;
        }
        else
        {
            m_moveSpeed = m_walkSpeed;

            //�d�ʂɂ���ăX�^�~�i�̑�������ύX�B
            StaminaWeightModifier(m_totalWeight, n);
            RecoveryStamina(m_moveStaminaRecovery * (1 / wightRatio));
        }

        //�X�y�[�X�������ꂽ��W�����v
        if (isGround == true && m_staminaGauge > 0.0f)
        {
            if (Input.GetButton("Jump"))
            {
                UseStamina(10.0f, 1.0f);
                m_rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGround = false;
            }
        }
        //�v���C���[�̑��x��ݒ肷�邱�Ƃňړ�������
        PlayerMove = (PlayerMove * m_moveSpeed * Time.deltaTime);
        PlayerMove.y = m_rigidBody.velocity.y;
        m_rigidBody.velocity = PlayerMove;

 
        
        if (stickL != Vector3.zero)
        {
            //�����p
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

    //�n�ʂ̐ڐG����
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    //�X�^�~�i�g�p�֐�
    void UseStamina(float stamina, float ratio)
    {
        m_staminaGauge -= stamina * Time.deltaTime * ratio;
    }

    //�X�^�~�i�񕜗p�֐�
    async void RecoveryStamina(float stamina)
    {
        //�X�^�~�i��0�ɂȂ�Ɖ񕜊J�n��x�点��
        if (m_staminaGauge <= 0)
        {
            m_staminaRecoveryFlag = false;
            //�����Ă��Ȃ��ꍇ�񕜂��J�n����
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

        //�X�^�~�i�������Ă�����񕜂���
        if (m_staminaGauge < 100.0f)
        {
            m_staminaGauge += stamina * Time.deltaTime;
            //�X�^�~�i������𒴂��Ă��������ɖ߂�
            if (m_staminaGauge > 100.0f)
            {
                m_staminaGauge = 100.0f;
            }
        }
    }

    //�X�^�~�i�̑��������v���C���[�̏d�ʂɂ���ĕύX����l������B
    void StaminaWeightModifier(float weight, float root)
    {
        wightRatio = (weight == 0.0f ? 1.0f : Mathf.Pow(weight, 1.0f / root));
    }

    //�v���C���[���A�C�e�����擾�������̏d���̉��Z�ƌ��Z�B
    //������true�̎��͎擾�Afalse�̎��͎̂Ă�B
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
        //�v���C���łȂ��Ȃ璆�f
        if (GameManager.GetGameState() != GameManager.GameState.enGameState_Play)
        {
            return;
        }

        PlayerStatus();
    }
}
