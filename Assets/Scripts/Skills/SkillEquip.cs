using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquip : MonoBehaviour
{
    public Image[] nowEquipImages;
    public TMP_Text[] nowEquipId;
    public Button[] equipBtns;
    public SkillManager skm;

    public SkillSO equipSo;

    private void OnEnable()
    {
        for (int i =0; i < 3; i++)
        {
            var s = skm.GetEquipped((SkillSlot)i);
            nowEquipImages[i].sprite = s.skillImage;
            nowEquipId[i].text = s.id;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i < 3; i++)
        {
            int idx = i;
            equipBtns[idx].onClick.AddListener(() =>
            {
                if(equipSo)
                {
                    skm.EquipSkill((SkillSlot)idx, equipSo.id);
                    equipSo = null;
                }


                PlayerController2D.inst.RefreshSkillUI();
                this.gameObject.SetActive(false);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
