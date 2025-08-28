using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Skills/Volley")]
public class VolleySO : SkillSO
{
    public Projectile proj;
    public int count = 5; public float spread = 18f, arc = 2.5f, flight = 0.7f, range = 8f;
    public override void OnFire(ref SkillCtx c)
    {
        if (!proj || !c.muzzle || !c.target) return;
        Vector2 dir = (c.target.position - c.muzzle.position).normalized;
        float baseAng = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        int half = count / 2;
        for (int i = -half; i <= half; i++)
        {
            float ang = baseAng + i * (spread / Mathf.Max(1, half));
            var p = UnityEngine.Object.Instantiate(proj, c.muzzle.position, Quaternion.Euler(0, 0, ang));
            p.ownerTeam = c.team;
            p.hitMask = LayerMask.GetMask("Default", "Enemy"); // 필요에 맞게
            Vector2 end = (Vector2)c.muzzle.position + new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad)) * range;
            p.Launch(c.muzzle.position, end, flight, arc);
        }
    }
}
