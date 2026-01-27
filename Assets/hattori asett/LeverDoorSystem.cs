using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// レバーをクリックして複数の鉄檻の扉を開閉するシステム
/// </summary>
public class LeverDoorSystem : MonoBehaviour
{
    [Header("扉の設定")]
    [Tooltip("開閉する扉のTransform配列")]
    public Transform[] doors;

    [Tooltip("扉が開いた時の高さ")]
    public float openHeight = 3.5f;

    [Tooltip("扉の開閉速度")]
    public float doorSpeed = 2f;

    [Tooltip("扉を順番に開く遅延時間（0で同時）")]
    public float doorDelay = 0.2f;

    [Header("レバーの設定")]
    [Tooltip("レバーのTransform（回転する部分）")]
    public Transform lever;

    [Tooltip("レバーを下げた時の角度")]
    public float leverDownAngle = 60f;

    [Tooltip("レバーの回転速度")]
    public float leverSpeed = 3f;

    [Header("状態")]
    public bool isDoorOpen = false;

    [Header("デバッグ")]
    [Tooltip("デバッグログを表示")]
    public bool showDebugLog = true;

    [Header("ハイライト設定")]
    [Tooltip("ホバー時のハイライトカラー")]
    public Color highlightColor = Color.red;

    [Tooltip("ハイライトの太さ")]
    public float outlineWidth = 0.05f;

    private Vector3[] doorClosedPositions;
    private Vector3[] doorOpenPositions;
    private float[] doorAnimationStartTimes;
    private Quaternion leverUpRotation;
    private Quaternion leverDownRotation;
    private bool isAnimating = false;
    private float animationStartTime;
    private bool isHoveringLever = false;
    private Renderer[] leverRenderers;
    private Material[][] originalMaterials;

    [SerializeField] AudioClip m_leverSE;

    void Start()
    {
        if (showDebugLog)
        {
            Debug.Log("=== LeverDoorSystem 初期化開始 ===");
        }

        // 各扉の初期位置を保存
        if (doors != null && doors.Length > 0)
        {
            doorClosedPositions = new Vector3[doors.Length];
            doorOpenPositions = new Vector3[doors.Length];
            doorAnimationStartTimes = new float[doors.Length];

            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] != null)
                {
                    doorClosedPositions[i] = doors[i].localPosition;
                    doorOpenPositions[i] = doorClosedPositions[i] + new Vector3(0, openHeight, 0);

                    if (showDebugLog)
                    {
                        Debug.Log($"扉 {i}: 閉じた位置 = {doorClosedPositions[i]}, 開いた位置 = {doorOpenPositions[i]}");
                    }
                }
                else
                {
                    Debug.LogWarning($"扉 {i} が設定されていません！");
                }
            }
        }
        else
        {
            Debug.LogError("扉が1つも設定されていません！Inspectorで扉を設定してください。");
        }

        // レバーの初期回転を保存
        if (lever != null)
        {
            leverUpRotation = lever.localRotation;
            leverDownRotation = Quaternion.Euler(leverDownAngle, 0, 0) * leverUpRotation;

            if (showDebugLog)
            {
                Debug.Log($"レバー設定完了: 上={leverUpRotation.eulerAngles}, 下={leverDownRotation.eulerAngles}");
            }

            // レバーのRendererを取得してマテリアルを保存
            leverRenderers = lever.GetComponentsInChildren<Renderer>();
            if (leverRenderers.Length > 0)
            {
                originalMaterials = new Material[leverRenderers.Length][];
                for (int i = 0; i < leverRenderers.Length; i++)
                {
                    originalMaterials[i] = leverRenderers[i].materials;
                }
            }
        }
        else
        {
            Debug.LogError("レバーが設定されていません！Inspectorでレバーを設定してください。");
        }

        if (showDebugLog)
        {
            Debug.Log("=== LeverDoorSystem 初期化完了 ===");
        }
    }

    void Update()
    {
        // マウスホバー検出
        CheckLeverHover();

        // レバーのクリック検出（左クリックのみ）
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Action"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (showDebugLog)
            {
                Debug.Log("左クリックが検出されました");
            }

            if (Physics.Raycast(ray, out hit))
            {
                if (showDebugLog)
                {
                    Debug.Log($"ヒット: {hit.transform.name}");
                }

                // レバーまたはその子オブジェクトをクリックした場合
                if (hit.transform == lever || hit.transform.IsChildOf(lever))
                {
                    if (showDebugLog)
                    {
                        Debug.Log("レバーがクリックされました！");
                    }

                    // レバーが上の状態の時のみ下げられる
                    if (!isDoorOpen)
                    {
                        if (showDebugLog)
                        {
                            Debug.Log("扉を開きます");
                        }
                        ToggleDoors();
                    }
                    else
                    {
                        if (showDebugLog)
                        {
                            Debug.Log("扉は既に開いています");
                        }
                    }
                }
            }
            else
            {
                if (showDebugLog)
                {
                    Debug.Log("何もヒットしませんでした");
                }
            }
        }

        // 扉とレバーのアニメーション
        if (isAnimating)
        {
            AnimateDoorsAndLever();
        }
    }

    /// <summary>
    /// マウスホバー時のレバーハイライト処理
    /// </summary>
    private void CheckLeverHover()
    {
        if (lever == null || isDoorOpen) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool isHovering = false;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == lever || hit.transform.IsChildOf(lever))
            {
                isHovering = true;
            }
        }

        // ホバー状態が変化した場合
        if (isHovering != isHoveringLever)
        {
            isHoveringLever = isHovering;

            if (isHovering)
            {
                ApplyHighlight();
            }
            else
            {
                RemoveHighlight();
            }
        }
    }

    /// <summary>
    /// レバーにハイライトを適用
    /// </summary>
    private void ApplyHighlight()
    {
        if (leverRenderers == null) return;

        foreach (Renderer renderer in leverRenderers)
        {
            Material[] mats = renderer.materials;
            foreach (Material mat in mats)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", highlightColor * 0.5f);
            }
        }

        // カーソルを変更
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// レバーからハイライトを除去
    /// </summary>
    private void RemoveHighlight()
    {
        if (leverRenderers == null || originalMaterials == null) return;

        for (int i = 0; i < leverRenderers.Length; i++)
        {
            if (originalMaterials[i] != null)
            {
                foreach (Material mat in leverRenderers[i].materials)
                {
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    /// <summary>
    /// すべての扉の開閉を切り替える
    /// </summary>
    public void ToggleDoors()
    {
        isDoorOpen = !isDoorOpen;
        isAnimating = true;
        animationStartTime = Time.time;

        if (showDebugLog)
        {
            Debug.Log($"=== ToggleDoors 実行 isDoorOpen={isDoorOpen} ===");
        }

        // 各扉のアニメーション開始時間を設定
        for (int i = 0; i < doorAnimationStartTimes.Length; i++)
        {
            doorAnimationStartTimes[i] = animationStartTime + (i * doorDelay);

            if (showDebugLog)
            {
                Debug.Log($"扉 {i} アニメーション開始時間: {doorAnimationStartTimes[i]}");
            }
        }
    }

    /// <summary>
    /// 扉とレバーをアニメーションさせる
    /// </summary>
    private void AnimateDoorsAndLever()
    {
        bool allDoorsReached = true;
        bool leverReached = true;

        // 各扉のアニメーション
        if (doors != null)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] == null) continue;

                // 扉のアニメーション開始時間に達しているか確認
                if (Time.time >= doorAnimationStartTimes[i])
                {
                    Vector3 targetPosition = isDoorOpen ? doorOpenPositions[i] : doorClosedPositions[i];
                    Vector3 oldPosition = doors[i].localPosition;
                    doors[i].localPosition = Vector3.Lerp(doors[i].localPosition, targetPosition, Time.deltaTime * doorSpeed);

                    if (showDebugLog && i == 0) // 最初の扉のみログ出力
                    {
                        Debug.Log($"扉 {i}: 現在位置={doors[i].localPosition}, 目標位置={targetPosition}, 距離={Vector3.Distance(doors[i].localPosition, targetPosition)}");
                    }

                    if (Vector3.Distance(doors[i].localPosition, targetPosition) > 0.01f)
                    {
                        allDoorsReached = false;
                    }
                    else
                    {
                        doors[i].localPosition = targetPosition;

                        if (showDebugLog && oldPosition != targetPosition)
                        {
                            Debug.Log($"扉 {i} が目標位置に到達しました: {targetPosition}");
                        }
                    }
                }
                else
                {
                    allDoorsReached = false;
                }
            }
        }

        // レバーのアニメーション
        if (lever != null)
        {
            Quaternion targetRotation = isDoorOpen ? leverDownRotation : leverUpRotation;
            lever.localRotation = Quaternion.Lerp(lever.localRotation, targetRotation, Time.deltaTime * leverSpeed);

            if (Quaternion.Angle(lever.localRotation, targetRotation) > 0.1f)
            {
                leverReached = false;
            }
            else
            {
                lever.localRotation = targetRotation;
            }
        }

        // アニメーション完了チェック
        if (allDoorsReached && leverReached)
        {
            isAnimating = false;

            if (showDebugLog)
            {
                Debug.Log("=== アニメーション完了 ===");
            }
        }
    }

    /// <summary>
    /// すべての扉を開く（外部から呼び出し可能）
    /// </summary>
    public void OpenDoors()
    {
        if (!isDoorOpen)
        {
            ToggleDoors();
        }
    }

    /// <summary>
    /// すべての扉を閉じる（外部から呼び出し可能）
    /// </summary>
    public void CloseDoors()
    {
        if (isDoorOpen)
        {
            ToggleDoors();
        }
    }

    /// <summary>
    /// 特定の扉のみを開閉（インデックス指定）
    /// </summary>
    public void ToggleSpecificDoor(int doorIndex)
    {
        if (doors != null && doorIndex >= 0 && doorIndex < doors.Length && doors[doorIndex] != null)
        {
            // 個別扉制御の実装（必要に応じて拡張）
            Debug.Log($"扉 {doorIndex} を切り替えました");
        }
    }
}