using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillShopCtrl : MonoBehaviour
{
    public SkillInfo skillinfoObj;
    public Button closeBtn;

    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
    }

    public void PressSkillInfoBtn(SkillSO so)
    {
        skillinfoObj.InitInfo(so);
        skillinfoObj.gameObject.SetActive(true);
    }
}
