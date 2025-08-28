using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour, IAnimEventReceiver
{
    public Animator anim;
    public Transform muzzle, target;
    public string team = "Player";
    public List<SkillSO> library;           // 인스펙터에 스킬 에셋 등록

    class SlotRT { public SkillSO so; public float cd; public bool casting; public string pendingId; }
    readonly Dictionary<SkillSlot, SlotRT> slots = new();
    readonly Dictionary<string, SkillSO> byId = new();
    SkillCtx ctx;

    void Awake()
    {
        foreach (var so in library) if (so) byId[so.id] = so;
        slots[SkillSlot.Q] = new SlotRT();
        slots[SkillSlot.W] = new SlotRT();
        slots[SkillSlot.E] = new SlotRT();
        ctx = new SkillCtx { self = transform, muzzle = muzzle, target = target, team = team, runner = this };
    }
    void Start()
    {
        EquipSkill(SkillSlot.Q, "Volley");
        //EquipSkill(SkillSlot.W, "Dash");
        //EquipSkill(SkillSlot.E, "Shield");
    }
    void Update()
    {
        foreach (var kv in slots) if (kv.Value.cd > 0) kv.Value.cd -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryCast(SkillSlot.Q);
        } 
        if (Input.GetKeyDown(KeyCode.W)) TryCast(SkillSlot.W);
        if (Input.GetKeyDown(KeyCode.E)) TryCast(SkillSlot.E);
    }

    public bool EquipSkill(SkillSlot slot, string id, bool force = false, bool resetCooldown = true)
    {
        if (!byId.TryGetValue(id, out var so)) return false;
        var r = slots[slot];
        if (r.casting && !force) { r.pendingId = id; return true; }
        r.so = so;
        if (resetCooldown) r.cd = 0f;         // 원하면 false로 유지
        r.pendingId = null;
        return true;
    }

    public bool TryCast(SkillSlot slot)
    {
        var r = slots[slot];
        if (r.so == null || r.casting || r.cd > 0) return false;
        r.casting = true;
        r.so.OnBegin(ref ctx);
        Debug.Log($"Slot_{slot}");
        anim.SetTrigger($"Slot_{slot}");       // Slot_Q/W/E 트리거
        return true;
    }

    // 애니메이션 이벤트 문자열: "Slot.Fire:Q" or "Slot.End:W"
    public void OnAnimEvent(string evt)
    {
        var parts = evt.Split(':'); if (parts.Length != 2) return;
        var tag = parts[0];
        if (!Enum.TryParse(parts[1], out SkillSlot slot)) return;
        var r = slots[slot]; if (r.so == null) return;

        if (tag == "Slot.Fire") r.so.OnFire(ref ctx);
        else if (tag == "Slot.End")
        {
            r.so.OnEnd(ref ctx);
            r.casting = false;
            r.cd = r.so.cooldown;
            if (!string.IsNullOrEmpty(r.pendingId)) EquipSkill(slot, r.pendingId, force: true);
        }
    }
}
