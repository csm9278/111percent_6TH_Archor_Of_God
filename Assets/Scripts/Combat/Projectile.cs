using UnityEngine;
public class Projectile : MonoBehaviour
{
    [Header("Damage/Team")]
    public int damage = 10;
    public string ownerTeam = "Player";
    public LayerMask hitMask;
    public float hitRadius = 0.12f;

    [Header("Motion")]
    public float duration = 0.6f;   // 전체 비행 시간
    public float arcHeight = 2.0f;  // 제어점의 상승 높이(+Y)
    public bool faceVelocity = true;

    Vector2 p0, p1, p2;   // start, control, end
    float t;
    bool active;
    Vector2 lastPos;

    public void Launch(Vector2 start, Vector2 end)
    {
        p0 = start;
        p2 = end;
        // 포물선 느낌의 제어점: 중점 + 위로 arcHeight
        Vector2 mid = (p0 + p2) * 0.5f;
        p1 = mid + Vector2.up * arcHeight;

        t = 0f;
        active = true;
        transform.position = p0;
        lastPos = p0;
    }

    void Update()
    {
        if (!active) return;

        t += Time.deltaTime / duration;
        float u = Mathf.Clamp01(t);

        Vector2 pos = Bezier(u);
        SweepHit(lastPos, pos);
        if (!active) return; // 충돌로 종료됐으면 더 진행하지 않음

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

        if (u >= 1f)
        {
            // 목적지에 정확히 도착
            active = false;
            Destroy(gameObject, 1.0f);
        }
    }

    Vector2 Bezier(float u)
    {
        float one = 1f - u;
        return one * one * p0 + 2f * one * u * p1 + u * u * p2; // 2차 베지어(포물선)
    }

    void SweepHit(Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from;
        float dist = dir.magnitude;
        if (dist <= 1e-6f) return;

        var hit = Physics2D.CircleCast(from, hitRadius, dir.normalized, dist, hitMask);
        if (hit.collider != null)
        {
            var d = hit.collider.GetComponent<Damageable>();
            if (d != null && d.team != ownerTeam)
            {
                d.ApplyDamage(damage);
                Destroy(gameObject, .02f);
            }
            transform.position = hit.point;
            active = false;
            Destroy(gameObject, .6f);
        }
    }
}
