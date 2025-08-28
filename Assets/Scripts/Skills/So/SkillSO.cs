using UnityEngine;

public struct SkillCtx
{
    public Transform self, muzzle, target;
    public string team;
    public MonoBehaviour runner;
}

public abstract class SkillSO : ScriptableObject
{
    public string id;            // "Volley","Dash","Shield" 등
    public float cooldown = 5f;
    public AnimationClip animClip; // 이 스킬 전용 클립
    public float fireTime = 0.3f;  // 0~1
    public virtual void OnBegin(ref SkillCtx ctx) { }
    public abstract void OnFire(ref SkillCtx ctx);
    public virtual void OnEnd(ref SkillCtx ctx) { }
}
