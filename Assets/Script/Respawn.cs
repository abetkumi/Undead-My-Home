using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerRespawn();
    }

    public void PlayerRespawn()
    {
        Rigidbody rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = gameObject.transform.position;
        if (gameObject == null) return;

        // 相手の水平角度（Y軸だけ）を取得
        float targetY = gameObject.transform.eulerAngles.y;

        // 現在のプレイヤー角度
        Vector3 currentEuler = rb.rotation.eulerAngles;

        // Y軸だけ置き換える
        Quaternion newRot = Quaternion.Euler(
            currentEuler.x,   // Xそのまま
            targetY,          // Yだけ相手と同じ
            currentEuler.z    // Zそのまま
        );

        rb.MoveRotation(newRot);
    }
}
