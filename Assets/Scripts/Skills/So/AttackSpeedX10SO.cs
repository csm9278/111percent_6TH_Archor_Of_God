// AttackSpeedX10SO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AttackSpeed x10 (5s)")]
public class AttackSpeedX10SO : SkillSO
{
    public float multiplier = 10f;
    public float buffDuration = 5f;

    public override void OnFire(ref SkillCtx ctx)
    {
        var host = ctx.self.GetComponent<AttackSpeedBuffRuntime>();
        if (!host) host = ctx.self.gameObject.AddComponent<AttackSpeedBuffRuntime>();
        host.Apply(multiplier, buffDuration);
    }
}
