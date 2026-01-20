using UnityEngine;
using System.Collections;

public class CollapsingBridge : MonoBehaviour
{
    [Header("崩壊設定")]
    [SerializeField] private float collapseDelay = 1.0f; // 崩れ始めるまでの遅延
    [SerializeField] private float collapseDuration = 2.0f; // 完全に崩れるまでの時間
    [SerializeField] private float shakeIntensity = 0.1f; // 揺れの強さ

    [Header("物理設定")]
    [SerializeField] private bool useGravity = true; // Rigidbodyを使うか
    [SerializeField] private float fallSpeed = 5.0f; // 落下速度（Rigidbody未使用時）

    private bool hasCollapsed = false;
    private bool isCollapsing = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();

        // Rigidbodyがあれば初期状態では固定
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーまたは指定したタグのオブジェクトが乗った時
        if (!hasCollapsed && other.CompareTag("Player"))
        {
            StartCoroutine(CollapseBridge());
        }
    }

    IEnumerator CollapseBridge()
    {
        hasCollapsed = true;

        // 遅延時間（この間に揺れる）
        float shakeTimer = 0f;
        while (shakeTimer < collapseDelay)
        {
            // ランダムな揺れ
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0
            );
            transform.position = originalPosition + shakeOffset;

            shakeTimer += Time.deltaTime;
            yield return null;
        }

        // 元の位置に戻す
        transform.position = originalPosition;
        isCollapsing = true;

        // Rigidbodyを使う場合
        if (useGravity && rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // 少しランダムな回転を加える
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }
        // Rigidbodyを使わない場合は自分で落下処理
        else
        {
            float elapsed = 0f;
            while (elapsed < collapseDuration)
            {
                transform.position += Vector3.down * fallSpeed * Time.deltaTime;
                transform.Rotate(Vector3.forward * 50f * Time.deltaTime);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 完全に崩れたら非表示または削除
            gameObject.SetActive(false);
        }
    }

    // リセット用（デバッグやリトライ機能用）
    public void ResetBridge()
    {
        StopAllCoroutines();
        hasCollapsed = false;
        isCollapsing = false;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}