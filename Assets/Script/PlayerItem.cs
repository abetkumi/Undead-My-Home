using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    GameObject m_cameraObject;  // メインカメラ
    GameObject m_hitObject;     // 選択中のオブジェクト

    const float SPHERE_RADIUS = 0.8f;           // SphereCastで発射する球体の半径
    const float SPHERE_MAX_DISTANCE = 16.0f;    // SphereCastで球体を発射する距離


    void Awake()
    {
        // メインカメラを取得する
        m_cameraObject = Camera.main.gameObject;
    }


    void Update()
    {
        // 球体を発射する
        RaycastHit hit;
        if (Physics.SphereCast(m_cameraObject.transform.position, SPHERE_RADIUS,
            m_cameraObject.transform.forward, out hit, SPHERE_MAX_DISTANCE))
        {
            // 今見ているオブジェクトと違う場合は選択終了
            if (m_hitObject != hit.collider.gameObject && m_hitObject != null)
            {
                EneSelect();
            }


            // 衝突したオブジェクトを取得
            ItemObject itemObject = hit.collider.gameObject.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                // 選択中の処理
                itemObject.StartSelect();
                m_hitObject = hit.collider.gameObject;


                // 決定時の処理
                if ((Input.GetKeyDown("joystick button 0") || Input.GetKeyDown(KeyCode.Return)))
                {
                    // アイテムに応じた処理
                    itemObject.ItemCheck();
                }


                // アイテム使用時の処理
                if ((Input.GetKeyDown("joystick button 2") || Input.GetKeyDown(KeyCode.I)))
                {
                    // アイテムに応じた処理
                    itemObject.ItemUse();
                }
            }

        }
        else
        {
            // どのオブジェクトにもヒットしていないので選択終了
            if (m_hitObject != null)
            {
                EneSelect();
            }
        }

    }


    // 選択終了
    void EneSelect()
    {
        m_hitObject.GetComponent<ItemObject>().EndSelect();
        m_hitObject = null;
    }
}
