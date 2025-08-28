// FireArrowProjectile.cs
using UnityEngine;
using System;

public class FireArrowProjectile : MonoBehaviour
{
    [Header("Damage/Team")]
    public int damage = 10;
    public string ownerTeam = "Player";
    public LayerMask hitMask;
    public float hitRadius = 0.12f;

    [Header("Bezier Motion")]
    public float duration = 0.6f;
    public float arcHeight = 2.0f;
    public bool faceVelocity = true;

    [Header("After Bezier")]
    public bool continuePastEnd = true;
    public float postSpeed = 12f;
    public float maxLife = 5f;

    // 콜백
    public Action<Collider2D, Vector2> onEnemyHit;
    public Action<Vector2, Vector2> onGroundHit; // point, normal

    Vector2 p0, p1, p2, lastPos, postDir;
    float t, life; bool active, bezierDone;

    public void Launch(Vector2 start, Vector2 end, float durationSec, float arc)
    {
        duration = Mathf.Max(0.01f, durationSec);
        arcHeight = arc;
        p0 = start; p2 = end; p1 = (p0 + p2) * 0.5f + Vector2.up * arc;
        t = 0; life = 0; active = true; bezierDone = false; postDir = Vector2.zero;
        transform.position = p0; lastPos = p0;
    }

    void Update()
    {
        if (!active) return;
        float dt = Time.deltaTime;
        life += dt; if (life >= maxLife) { Destroy(gameObject); return; }

        if (!bezierDone)
        {
            t += dt / duration;
            float u = Mathf.Clamp01(t);
            Vector2 pos = Bezier(u);
            SweepHit(lastPos, pos); if (!active) return;
            FaceMove(pos);
            if (u >= 1f)
            {
                bezierDone = true;
                postDir = (p2 - p1).sqrMagnitude > 1e-6f ? (p2 - p1).normalized : (pos - p1).normalized;
            }
        }
        else
        {
            Vector2 pos = (Vector2)transform.position + postDir * postSpeed * dt;
            SweepHit(lastPos, pos); if (!active) return;
            FaceMove(pos);
        }
    }

    Vector2 Bezier(float u)
    {
        float one = 1f - u;
        return one * one * p0 + 2f * one * u * p1 + u * u * p2;
    }

    void FaceMove(Vector2 pos)
    {
        if (faceVelocity)
        {
            Vector2 v = pos - lastPos;
            if (v.sqrMagnitude > 1e-6f)
            {
                float ang = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, ang - 90);
            }
        }
        transform.position = pos;
        lastPos = pos;
    }

    void SweepHit(Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from; float dist = dir.magnitude;
        if (dist <= 1e-6f) return;

        var hit = Physics2D.CircleCast(from, hitRadius, dir.normalized, dist, hitMask);
        if (!hit.collider) return;

        // Enemy or Ground 판별: Damageable 유무로 분기
        var dmg = hit.collider.GetComponentInParent<Damageable>();
        if (dmg && dmg.team != ownerTeam)
        {
            onEnemyHit?.Invoke(hit.collider, hit.point);
        }
        else
        {
            onGroundHit?.Invoke(hit.point, hit.normal);
        }

        transform.position = hit.point;
        active = false;
        Destroy(gameObject, .02f);
    }
}
