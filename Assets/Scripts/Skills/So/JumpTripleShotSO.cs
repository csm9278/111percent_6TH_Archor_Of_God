// JumpTripleShotSO.cs
using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skills/JumpTripleShot")]
public class JumpTripleShotSO : SkillSO
{
    public Projectile projPrefab;
    public LayerMask hitMask;
    public float range = 8f, flight = 0.6f, arc = 2.0f;

    // 3발 간격 (초 단위)
    public float interval = 0.1f;

    public override void OnFire(ref SkillCtx ctx)
    {
        ctx.runner.StartCoroutine(FireThree(ctx));
    }

    IEnumerator FireThree(SkillCtx ctx)
    {
        for (int i = 0; i < 3; i++)
        {
            FireOne(ctx);
            yield return new WaitForSeconds(interval);
        }
    }

    void FireOne(SkillCtx ctx)
    {
        if (!projPrefab || !ctx.muzzle) return;
        Vector2 dir = ctx.target
            ? ((Vector2)ctx.target.position - (Vector2)ctx.muzzle.position).normalized
            : Vector2.right;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var p = Object.Instantiate(projPrefab, ctx.muzzle.position, Quaternion.Euler(0, 0, ang));
        p.ownerTeam = ctx.team;
        p.hitMask = LayerMask.GetMask("Player","Ground","Enemy");
        p.faceVelocity = true;
        p.continuePastEnd = true;

        Vector2 end = (Vector2)ctx.muzzle.position + dir * range;
        p.Launch(ctx.muzzle.position, end, flight, arc);
    }
}

