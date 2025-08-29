// DamageTextUIBezier.cs
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text text;

    [Header("Motion")]
    public float duration = 0.8f;      // 전체 재생 시간
    public Vector2 startOffset = new(0f, 100f);   // 월드 Y+1 → 대략 100px
    public Vector2 ctrlOffset = new(0f, 150f);   // 월드 Y+1.5 → 대략 150px
    public Vector2 endOffset = new(0f, 0f);     // 월드 Y

    [Header("Style")]
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 1, 1, 1);

    RectTransform rt;
    Vector2 p0, p1, p2;         // UI 좌표(Bézier)
    float t;
    Color baseColor;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (!text) text = GetComponent<TMP_Text>();
        baseColor = text ? text.color : Color.white;
    }

    /// <summary>
    /// worldPos = 피격 대상의 world 위치, canvas = Screen Space Canvas
    /// </summary>
    public void Init(string dmg, Vector3 worldPos, Canvas canvas, Camera worldCam = null)
    {
        if (text) text.text = dmg;

        // Overlay면 null, Camera 모드면 worldCamera 사용
        var camForCanvas = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        worldCam ??= Camera.main;
        var canvasRect = (RectTransform)canvas.transform;

        // 월드→스크린
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(worldCam, worldPos);

        // 스크린→UI 로컬 좌표
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, camForCanvas, out var ui);

        // 베지어 점 정의(요구사항: 시작=Y+1, 제어=Y+1.5, 끝=Y)
        p0 = ui + startOffset;   // 위에서 시작
        ctrlOffset = new Vector2(Random.Range(-50, 50), 200f);
        endOffset = new Vector2(ctrlOffset.x > 0 ? ctrlOffset.x + 50f : ctrlOffset.x - 50f, 0f);
        p1 = ui + ctrlOffset;    // 더 위로 살짝
        p2 = ui + endOffset;     // 아래(입은 객체 Y)

        // 초기화
        t = 0f;
        if (text) text.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        rt.anchoredPosition = p0;
        rt.localScale = Vector3.one;
    }

    void Update()
    {
        if (t >= duration) { Destroy(gameObject); return; }
        t += Time.unscaledDeltaTime;
        float u = Mathf.Clamp01(t / duration);

        // 2차 베지어
        Vector2 pos = QuadBezier(p0, p1, p2, u);
        rt.anchoredPosition = pos;

        // alpha/scale
        float a = alphaCurve.Evaluate(u);
        if (text) text.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
        float s = scaleCurve.Evaluate(u);
        rt.localScale = new Vector3(s, s, 1f);
    }

    static Vector2 QuadBezier(Vector2 a, Vector2 b, Vector2 c, float u)
    {
        float one = 1f - u;
        return one * one * a + 2f * one * u * b + u * u * c;
    }
}
