using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 指定した秒数後に画面を徐々に暗くするスクリプト
/// </summary>
public class ScreenFadeOut : MonoBehaviour
{
    [Header("フェード設定")]
    [Tooltip("暗くなり始めるまでの待機時間（秒）")]
    public float delayBeforeFade = 3f;

    [Tooltip("フェードアウトにかかる時間（秒）")]
    public float fadeDuration = 2f;

    [Tooltip("フェード後の最終的な暗さ（0=完全に黒, 1=変化なし）")]
    [Range(0f, 1f)]
    public float targetAlpha = 1f;

    private Image fadeImage;

    void Start()
    {
        SetupFadeImage();
        StartCoroutine(FadeOutRoutine());
    }

    /// <summary>
    /// フェード用の黒いImageを自動生成する
    /// </summary>
    private void SetupFadeImage()
    {
        // Canvas を作成（または既存のものを探す）
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FadeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // 最前面に表示
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 黒いパネルを作成
        GameObject panel = new GameObject("FadePanel");
        panel.transform.SetParent(canvas.transform, false);

        fadeImage = panel.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // 最初は透明

        // 画面全体を覆うように設定
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// 待機 → フェードアウトのコルーチン
    /// </summary>
    private IEnumerator FadeOutRoutine()
    {
        // 指定秒数待機
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, targetAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 最終値に確定
        fadeImage.color = new Color(0, 0, 0, targetAlpha);

        OnFadeComplete();
    }

    /// <summary>
    /// フェード完了後の処理（必要に応じてオーバーライドまたは編集）
    /// </summary>
    private void OnFadeComplete()
    {
        Debug.Log("フェードアウト完了");
        // 例: シーン遷移する場合
        // UnityEngine.SceneManagement.SceneManager.LoadScene("NextScene");
    }

    /// <summary>
    /// 外部から手動でフェードを開始する場合に呼ぶ
    /// </summary>
    public void StartFade()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }
}