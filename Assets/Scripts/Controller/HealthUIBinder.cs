using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthUIBinder : MonoBehaviour
{
    public Health health;        // 대상
    public Image bar;            // Image Type=Filled, Fill Method=Horizontal
    public TMP_Text label;       // "90/100" 같은 표기

    void OnEnable()
    {
        if (health != null)
        {
            health.OnChanged += UpdateUI;
            UpdateUI(health.Current, health.maxHP); // 초기 반영
        }
    }
    void OnDisable()
    {
        if (health != null) health.OnChanged -= UpdateUI;
    }
    void UpdateUI(int cur, int max)
    {
        if (bar) bar.fillAmount = max > 0 ? (float)cur / max : 0f;
        if (label) label.text = $"{cur}/{max}";
    }
}
