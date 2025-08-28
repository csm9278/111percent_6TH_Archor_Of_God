using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthUIBinder : MonoBehaviour
{
    public Health health;        // ���
    public Image bar;            // Image Type=Filled, Fill Method=Horizontal
    public TMP_Text label;       // "90/100" ���� ǥ��

    void OnEnable()
    {
        if (health != null)
        {
            health.OnChanged += UpdateUI;
            UpdateUI(health.Current, health.maxHP); // �ʱ� �ݿ�
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
