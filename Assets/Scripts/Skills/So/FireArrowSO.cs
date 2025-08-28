// FireArrowSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/FireArrow")]
public class FireArrowSO : SkillSO
{
    public FireArrowProjectile projPrefab;         // ��ȭ�� ������
    public LayerMask hitMask;                      // Enemy, Ground ����
    public float flight = 0.6f, arc = 2.0f, range = 9f;

    [Header("On Enemy Hit �� Burn DoT")]
    public float burnDuration = 3f;                // ��
    public float burnTick = 0.5f;                  // ��
    public int burnDamagePerTick = 3;

    [Header("On Ground Hit �� Fire Area")]
    public FireArea areaPrefab;                    // ȭ������ ������
    public float areaRadius = 1.5f;
    public float areaDuration = 4f;
    public float areaTick = 0.5f;
    public int areaDamagePerTick = 2;

    public override void OnBegin(ref SkillCtx ctx)
    {
        PlayerController2D.inst.skillCasting = true;
    }

    public override void OnFire(ref SkillCtx c)
    {
        if (!projPrefab || !c.muzzle) return;

        // ref�� ���� c�� ���� ������ ����
        var ctx = c;
        Vector2 dir = ((ctx.target ? ctx.target.position : ctx.self.position + ctx.self.right) - ctx.muzzle.position).normalized;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var p = Object.Instantiate(projPrefab, ctx.muzzle.position, Quaternion.Euler(0, 0, ang));
        p.ownerTeam = ctx.team;
        p.hitMask = LayerMask.GetMask("Ground", "Enemy", "Player"); // �ʿ信 �°�

        // Enemy Hit
        p.onEnemyHit = (col, hitPt) => {
            var d = col.GetComponentInParent<Damageable>();
            if (d)
            {
                d.ApplyDamage(p.damage);
                var area = Object.Instantiate(areaPrefab, col.gameObject.transform.position, Quaternion.identity);
                area.gameObject.transform.SetParent(col.gameObject.transform);
                var burn = d.GetComponent<BurnDoT>() ?? d.gameObject.AddComponent<BurnDoT>();
                burn.Apply(burnDuration, burnTick, burnDamagePerTick);
            }
        };

        // Ground Hit
        p.onGroundHit = (hitPt, normal) => {
            if (!areaPrefab) return;
            var area = Object.Instantiate(areaPrefab, hitPt, Quaternion.identity);
            area.Init(ctx.team, areaRadius, areaDuration, areaTick, areaDamagePerTick);
        };

        // Launch
        p.Launch(ctx.muzzle.position, ctx.target.position, flight, arc);
    }

    public override void OnEnd(ref SkillCtx ctx)
    {
        PlayerController2D.inst.skillCasting = false;
    }
}
