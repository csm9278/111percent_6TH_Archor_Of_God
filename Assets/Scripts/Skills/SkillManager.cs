using UnityEngine;
using System.Collections.Generic;

public enum SkillSlot { Q = 0, W = 1, E = 2 }

public class SkillManager : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;
    public AnimationClip slotBase;             
    public Transform muzzle, target;
    public string team = "Player";

    [Header("Library")]
    public List<SkillSO> library;               

    class SlotRT { public SkillSO so; public float cd; public bool casting; public string pendingId; }
    Dictionary<SkillSlot, SlotRT> slots = new Dictionary<SkillSlot, SlotRT>();
    Dictionary<string, SkillSO> byId = new Dictionary<string, SkillSO>();
    AnimatorOverrideController aoc;

    public SkillUI[] skillUIs;
    void Awake()
    {
        aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = aoc;

        foreach (var so in library) if (so) byId[so.id] = so;

        slots[SkillSlot.Q] = new SlotRT();
        slots[SkillSlot.W] = new SlotRT();
        slots[SkillSlot.E] = new SlotRT();
    }

    private void Start()
    {
        EquipSkill(SkillSlot.Q, "Volley");
        EquipSkill(SkillSlot.W, "FireArrow");
        EquipSkill(SkillSlot.E, "AttackSpeedX10");
    }

    void Update()
    {
        if (!GameManager.inst.gameStart)
            return;

        // 쿨다운 감소
        foreach (var kv in slots)
        {
            if (kv.Value.cd > 0)
            {
                kv.Value.cd -= Time.deltaTime;
            }
        }

        if (team != "Player")
            return;

        if (PlayerController2D.inst.skillCasting)
            return;
        if (Input.GetKeyDown(KeyCode.Q)) TryCast(SkillSlot.Q);
        if (Input.GetKeyDown(KeyCode.W)) TryCast(SkillSlot.W);
        if (Input.GetKeyDown(KeyCode.E)) TryCast(SkillSlot.E);
    }

    public bool EquipSkill(SkillSlot slot, string id, bool force = false, bool resetCooldown = true)
    {
        if (!byId.TryGetValue(id, out var so)) return false;
        var r = slots[slot];
        if (r.casting && !force) { r.pendingId = id; return true; }
        r.so = so;
        if (resetCooldown) r.cd = 0f;
        r.pendingId = null;
        return true;
    }

    public bool TryCast(SkillSlot slot)
    {
        var r = slots[slot];
        if (r.so == null || r.casting || r.cd > 0) return false;

        // 애니메이션 클립 교체
        if (slotBase && r.so.animClip) aoc[slotBase] = r.so.animClip;

        // 슬롯 전달 + 트리거
        anim.SetInteger("Slot", (int)slot);   
        anim.ResetTrigger("Skill");
        anim.SetTrigger("Skill");             

        r.casting = true;
        var ctx = GetCtx();
        return true;
    }

    public SkillSO GetEquipped(SkillSlot slot)
    {
        return slots.TryGetValue(slot, out var r) ? r.so : null;
    }

    public SkillCtx GetCtx()
    {
        return new SkillCtx { self = transform, muzzle = muzzle, target = target, team = team, runner = this };
    }

    public void OnSkillEnd(SkillSlot slot)
    {
        var r = slots[slot];
        r.casting = false;
        if (r.so != null) r.cd = r.so.cooldown;
        if (!string.IsNullOrEmpty(r.pendingId)) EquipSkill(slot, r.pendingId, force: true);
    }

    // UI용 상태 조회
    public (string id, float cdRemain, bool casting) GetSlotState(SkillSlot slot)
    {
        var r = slots[slot];
        return (r.so ? r.so.id : null, Mathf.Max(0, r.cd), r.casting);
    }
}
