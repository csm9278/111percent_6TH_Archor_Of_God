using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillVolley : SkillBase
{
    public Projectile proj;
    public Transform muzzle; public Transform target;
    public int count = 5; public float spread = 18f; public float arc = 2.5f; public float flight = 0.7f;
    public string team;

    protected override void OnFire()
    {
        if (!proj || !muzzle || !target) return;
        Vector2 dir = (target.position - muzzle.position).normalized;
        float baseAng = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        int half = count / 2;
        for (int i = -half; i <= half; i++)
        {
            float ang = baseAng + i * (spread / Mathf.Max(1, half));
            var p = Instantiate(proj, muzzle.position, Quaternion.Euler(0, 0, ang));
            p.ownerTeam = team;
            Vector2 end = (Vector2)muzzle.position + new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad)) * 8f;
            p.Launch(muzzle.position, end, flight, arc);
        }
    }
}
