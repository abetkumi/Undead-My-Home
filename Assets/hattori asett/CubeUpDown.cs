using UnityEngine;

public class CubeUpDown : MonoBehaviour
{
    // 移動の速度
    public float speed = 2f;

    // 移動の範囲
    public float range = 2f;

    // 初期位置
    private Vector3 startPosition;

    void Start()
    {
        // 初期位置を保存
        startPosition = transform.position;
    }

    void Update()
    {
        // Sin波を使って滑らかな上下移動
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * range;

        // Y座標のみを変更
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }
}
