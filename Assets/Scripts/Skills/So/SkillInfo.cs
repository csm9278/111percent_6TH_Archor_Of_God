using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    public TMP_Text skillNameTmp;
    public TMP_Text skillInfo;
    public Image skillImage;
    public Button EquipBtn;
    public Button CancleBtn;

    private void Start()
    {
        CancleBtn.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });

        EquipBtn.onClick.AddListener(() =>
        {

        });
    }

    public void InitInfo(SkillSO so)
    {
        skillNameTmp.text = so.id;
        skillImage.sprite = so.skillImage;
        skillInfo.text = so.skillInfo;
        skillInfo.text += "\n" + "Äð´Ù¿î: " + so.cooldown;
    }

}
