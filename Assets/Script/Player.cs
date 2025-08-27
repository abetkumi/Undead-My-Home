using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] float m_moveSpeed = 500.0f;
    float m_walkSpeed = 500.0f;
    float m_runSpeed = 1000.0f;
    float t = 0.5f;
    bool isGround = true;
    public float jumpForce = 50.0f;
    PlayerState m_playerState = PlayerState.Idle;

    //�L���b�V��
    Rigidbody m_rigidBody;
    GameObject m_gameCameraObj;
    GameObject m_playerObject;

    // Start is called before the first frame update
    void Start()
    {
        //�K�v�ȏ����擾
        m_rigidBody = GetComponent<Rigidbody>();
        m_gameCameraObj = Camera.main.gameObject;
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
                break;
        }
    }

    void Idle()
    {
        if (Input.anyKey)
        {
            m_playerState = PlayerState.Move;
        }
    }

    void Move()
    {
        //�J�������l�������ړ�
        Vector3 PlayerMove = Vector3.zero;
        Vector3 stickL = Vector3.zero;
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
        if (Input.GetButton("Run"))
        {
            m_moveSpeed = m_runSpeed;
        }
        else
        {
            m_moveSpeed = m_walkSpeed;
        }

        //�X�y�[�X�������ꂽ��W�����v
        if (isGround == true)
        {
            if (Input.GetButton("Jump"))
            {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
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
