using UnityEngine;

public struct SkillCtx
{
    public Transform self, muzzle, target;
    public string team;
    public MonoBehaviour runner;
}

public abstract class SkillSO : ScriptableObject
{
    public string id;            // "Volley","Dash","Shield" ��
    public float cooldown = 5f;
    public AnimationClip animClip; // �� ��ų ���� Ŭ��
    public float fireTime = 0.3f;  // 0~1
    public virtual void OnBegin(ref SkillCtx ctx) { }
    public abstract void OnFire(ref SkillCtx ctx);
    public virtual void OnEnd(ref SkillCtx ctx) { }
}
