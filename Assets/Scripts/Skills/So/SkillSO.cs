using UnityEngine;

public enum SkillSlot { Q, W, E }

public struct SkillCtx
{
    public Transform self, muzzle, target;
    public string team;
    public MonoBehaviour runner;
}

public abstract class SkillSO : ScriptableObject
{
    public string id;            // "Volley","Dash","Shield" µî
    public float cooldown = 5f;

    public virtual void OnBegin(ref SkillCtx ctx) { }
    public abstract void OnFire(ref SkillCtx ctx);
    public virtual void OnEnd(ref SkillCtx ctx) { }
}
