using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;

    // フォーカス処理
    Vector3 m_targetPos, m_startPos;
    float m_focusTimer = 0.0f;
    float m_speed;
    bool m_isFocus = false;
    public void FocusStart(Vector3 targetPos, float speed, float range = 10.0f)
    {
        // フォーカス準備
        m_startPos = transform.position + (transform.forward * range);
        m_targetPos = targetPos;
        m_speed = speed;
        m_focusTimer = 0.0f;
        m_isFocus = true;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // フォーカス処理
        if (m_isFocus == false)
        {
            return;
        }

        m_focusTimer += Time.deltaTime * m_speed;
        Vector3 nowTarget = Vector3.Lerp(m_startPos, m_targetPos, m_focusTimer);

        // 対象を見る
        transform.LookAt(nowTarget);
        if (m_focusTimer > 1.0f)
        {
            // フォーカス終了
            m_isFocus = false;
        }
    }

    void LateUpdate()
    {
        //プレイ中でないなら中断
        if (GameManager.GetGameState() != GameManager.GameState.enGameState_Play)
        {
            return;
        }

        float mouseX = Input.GetAxis("Camera_Horizontal") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Camera_Vertical") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}