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

    //パラメータ
    [SerializeField] float m_moveSpeed = 500.0f;
    float m_walkSpeed = 500.0f;
    float m_runSpeed = 1000.0f;
    float t = 0.5f;
    bool isGround = true;
    public float jumpForce = 50.0f;
    PlayerState m_playerState = PlayerState.Idle;

    //キャッシュ
    Rigidbody m_rigidBody;
    GameObject m_gameCameraObj;
    GameObject m_playerObject;

    // Start is called before the first frame update
    void Start()
    {
        //必要な情報を取得
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
        //カメラを考慮した移動
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

        //移動速度に上記で計算したベクトルを加算する
        PlayerMove += right + forward;

        //Runキーが押されている場合
        if (Input.GetButton("Run"))
        {
            m_moveSpeed = m_runSpeed;
        }
        else
        {
            m_moveSpeed = m_walkSpeed;
        }

        //スペースが押されたらジャンプ
        if (isGround == true)
        {
            if (Input.GetButton("Jump"))
            {
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
        //プレイ中でないなら中断
        if (GameManager.GetGameState() != GameManager.GameState.enGameState_Play)
        {
            return;
        }

        PlayerStatus();
    }
}
