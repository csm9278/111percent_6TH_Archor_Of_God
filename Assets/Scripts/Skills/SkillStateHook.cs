using UnityEngine;

public class SkillStateHook : StateMachineBehaviour
{
    bool fired;
    static readonly int SlotHash = Animator.StringToHash("Slot"); // Int �Ķ����

    public override void OnStateEnter(Animator a, AnimatorStateInfo s, int l)
    {
        fired = false;
        var sm = a.GetComponent<SkillManager>();
        if (!sm) return;
        var slot = (SkillSlot)a.GetInteger(SlotHash);
        var so = sm.GetEquipped(slot);
        if (so == null) return;

        var ctx = sm.GetCtx();
        so.OnBegin(ref ctx);
    }

    public override void OnStateUpdate(Animator a, AnimatorStateInfo s, int l)
    {
        var sm = a.GetComponent<SkillManager>();
        if (!sm) return;
        var slot = (SkillSlot)a.GetInteger(SlotHash);
        var so = sm.GetEquipped(slot);
        if (so == null) return;

        // ���� ���: normalizedTime%1f ���. ���°� ������� s.normalizedTime �״�ε� OK.
        float t = s.normalizedTime % 1f;
        if (!fired && t >= so.fireTime)
        {
            fired = true;
            var ctx = sm.GetCtx();
            so.OnFire(ref ctx);
        }
    }

    public override void OnStateExit(Animator a, AnimatorStateInfo s, int l)
    {
        var sm = a.GetComponent<SkillManager>();
        if (!sm) return;
        var slot = (SkillSlot)a.GetInteger(SlotHash);
        var so = sm.GetEquipped(slot);
        if (so != null)
        {
            var ctx = sm.GetCtx();
            so.OnEnd(ref ctx);
        }
        sm.OnSkillEnd(slot);
    }
}
