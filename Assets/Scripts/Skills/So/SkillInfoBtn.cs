using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoBtn : MonoBehaviour
{
    public SkillSO so;
    public Image skillIcon;
    public TMP_Text skillName;
    Button btn;
    SkillShopCtrl ssc;
    // Start is called before the first frame update
    void Awake()
    {
        ssc = GetComponentInParent<SkillShopCtrl>();
        btn = this.GetComponent<Button>();
        skillIcon.sprite = so.skillImage;
        skillName.text = so.id;

        btn.onClick.AddListener(() =>
        {
            ssc.PressSkillInfoBtn(so);
        });
    }
}
