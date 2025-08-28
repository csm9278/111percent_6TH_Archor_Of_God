using UnityEngine;

public abstract class SkillBase : MonoBehaviour, IAnimEventReceiver
{
    public string triggerName = "SkillQ";
    public float cooldown = 5f;
    protected float cdRemain;
    protected bool casting;
    protected Animator anim;
    protected ActionGate gate;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        gate = GetComponent<ActionGate>();
    }
    void Update() { if (cdRemain > 0) cdRemain -= Time.deltaTime; }

    public bool TryCast()
    {
        if (cdRemain > 0 || casting || (gate != null && gate.Locked)) return false;
        casting = true; gate?.Lock();
        anim.SetTrigger(triggerName);
        return true;
    }

    public void OnAnimEvent(string evt)
    {
        if (evt == triggerName + "_Fire") OnFire();
        else if (evt == triggerName + "_End") { casting = false; gate?.Release(); cdRemain = cooldown; }
    }

    protected abstract void OnFire(); // 실제 효과
    public float Cooldown => cooldown;
    public float Remain => Mathf.Max(0, cdRemain);
}
