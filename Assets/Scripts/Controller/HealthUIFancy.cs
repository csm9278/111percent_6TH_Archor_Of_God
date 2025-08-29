using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthUIFancy : MonoBehaviour
{
    [Header("Refs")]
    public Health health;
    public Image frontBar;             
    public Image chipBar;              
    public TMP_Text label;

    [Header("Anim")]
    public float frontLerpSpeed = 12f; 
    public float chipDelay = 0.15f;    
    public float chipLerpSpeed = 2.5f; 
    public Color healColor = new(0.6f, 1f, 0.6f, 1f);
    public Color dmgColor = new(1f, 0.6f, 0.6f, 1f);
    public float flashTime = 0.12f;

    float targetFill;                   
    Coroutine chipCo, flashCo;

    void OnEnable()
    {
        if (!health) return;
        health.OnChanged += OnChanged;
        // 초기화
        targetFill = Mathf.Clamp01((float)health.Current / health.maxHP);
        SetFill(frontBar, targetFill);
        SetFill(chipBar, targetFill);
        UpdateText(health.Current, health.maxHP);
    }
    void OnDisable() { if (health) health.OnChanged -= OnChanged; }

    void Update()
    { // front 즉시 보간
        if (!frontBar) return;
        float cur = frontBar.fillAmount;
        if (Mathf.Approximately(cur, targetFill)) return;
        cur = Mathf.MoveTowards(cur, targetFill, frontLerpSpeed * Time.unscaledDeltaTime);
        SetFill(frontBar, cur);
    }

    void OnChanged(int cur, int max)
    {
        float newFill = max > 0 ? (float)cur / max : 0f;
        bool tookDamage = newFill < targetFill;
        targetFill = newFill;
        UpdateText(cur, max);

        if (chipCo != null) StopCoroutine(chipCo);
        chipCo = StartCoroutine(ChipRoutine(tookDamage));

        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(FlashRoutine(tookDamage));
    }

    IEnumerator ChipRoutine(bool tookDamage)
    {
        if (!chipBar) yield break;

        if (tookDamage)
        {
            // 잠깐 대기 후 천천히 칩 감소
            yield return new WaitForSecondsRealtime(chipDelay);
            while (!Mathf.Approximately(chipBar.fillAmount, targetFill))
            {
                float cur = Mathf.MoveTowards(chipBar.fillAmount, targetFill, chipLerpSpeed * Time.unscaledDeltaTime);
                SetFill(chipBar, cur);
                yield return null;
            }
        }
        else
        {
            while (!Mathf.Approximately(chipBar.fillAmount, targetFill))
            {
                float cur = Mathf.MoveTowards(chipBar.fillAmount, targetFill, (chipLerpSpeed * 1.5f) * Time.unscaledDeltaTime);
                SetFill(chipBar, cur);
                yield return null;
            }
        }
    }

    IEnumerator FlashRoutine(bool tookDamage)
    {
        if (!frontBar) yield break;
        var orig = frontBar.color;
        frontBar.color = tookDamage ? dmgColor : healColor;
        yield return new WaitForSecondsRealtime(flashTime);
        frontBar.color = orig;
    }

    void SetFill(Image img, float v) { if (img) img.fillAmount = Mathf.Clamp01(v); }
    void UpdateText(int cur, int max) { if (label) label.text = $"{cur}/{max}"; }
}
