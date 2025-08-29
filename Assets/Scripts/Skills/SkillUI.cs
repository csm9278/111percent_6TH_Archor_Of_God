using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [Header("Refs")]
    public SkillManager skillManager;
    public SkillSlot slot;

    public Image fillImage;
    public TMP_Text nameText;
    public TMP_Text cdText;

    public Image icon;

    [Header("Visual")]
    public bool dimIconWhileCooling = true;
    [Range(0f, 1f)] public float dimAlpha = 0.5f;

    void Reset()
    {
        fillImage = GetComponent<Image>();
        cdText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        if (!skillManager) return;

        var so = skillManager.GetEquipped(slot);
        if (so == null)
        {
            if (fillImage) fillImage.fillAmount = 0f;
            if (cdText) cdText.text = "";
            if (icon) SetIconAlpha(1f);
            return;
        }

        // 남은 쿨다운/총 쿨다운
        var state = skillManager.GetSlotState(slot);
        float remain = Mathf.Max(0f, state.cdRemain);
        float total = Mathf.Max(0.0001f, so.cooldown);

        // 오버레이 채우기(남은 비율)
        if (fillImage) fillImage.fillAmount = remain / total;

        // 숫자 표시(정수 초, 0이면 숨김)
        if (cdText) cdText.text = remain > 0.05f ? remain.ToString("F1") : "";

        // 아이콘 디밍
        if (icon) SetIconAlpha(remain > 0f && dimIconWhileCooling ? dimAlpha : 1f);
    }

    void SetIconAlpha(float a)
    {
        var c = icon.color; c.a = a; icon.color = c;
    }

    void SetSkill(SkillSO so)
    {
        icon.sprite = so.skillImage;
        fillImage.sprite = so.skillImage;
        nameText.text = so.id;
    }
}
