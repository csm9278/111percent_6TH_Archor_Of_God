using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Damage/Team")]
    public int damage = 10;
    public string ownerTeam = "Player";
    public LayerMask hitMask;
    public float hitRadius = 0.12f;

    [Header("Bezier Motion")]
    public float duration = 0.6f;   // 곡선 구간 시간
    public float arcHeight = 2.0f;  // 제어점 상승 높이(+Y)
    public bool faceVelocity = true;

    [Header("After Bezier")]
    public bool continuePastEnd = true;  // 끝나고도 비행
    public float postSpeed = 12f;        // 직진 속도
    public float maxLife = 5f;           // 안전차단(누수 방지)

    Vector2 p0, p1, p2;                  // start, control, end
    float t;                             // 0..1
    bool active;                         // 충돌 시에만 false
    bool bezierDone;                     // 베지어 구간 종료 여부
    Vector2 lastPos;
    Vector2 postDir;                     // 베지어 종단 접선
    float life;

    public void Launch(Vector2 start, Vector2 end, float durationSec, float arc)
    {
        duration = Mathf.Max(0.01f, durationSec);
        arcHeight = arc;

        p0 = start;
        p2 = end;
        Vector2 mid = (p0 + p2) * 0.5f;
        p1 = mid + Vector2.up * arcHeight;

        t = 0f;
        life = 0f;
        active = true;
        bezierDone = false;
        postDir = Vector2.zero;

        transform.position = p0;
        lastPos = p0;
    }

    void Update()
    {
        if (!active) return;

        float dt = Time.deltaTime;
        life += dt;
        if (life >= maxLife) { Destroy(gameObject); return; }

        Vector2 pos;

        if (!bezierDone)
        {
            t += dt / duration;
            float u = Mathf.Clamp01(t);
            pos = Bezier(u);

            // 먼저 이동 경로 충돌 체크
            SweepHit(lastPos, pos);
            if (!active) return;

            // 회전 처리
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

            // 베지어 종료 → 접선 방향으로 전환
            if (u >= 1f)
            {
                bezierDone = true;
                Vector2 tangent = (p2 - p1); // 2차 베지어 종단 접선
                if (tangent.sqrMagnitude < 1e-6f) tangent = (pos - p1);
                postDir = tangent.normalized;
            }
        }
        else
        {
            // 직진 구간
            pos = (Vector2)transform.position + postDir * postSpeed * dt;

            SweepHit(lastPos, pos);
            if (!active) return;

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
    }

    Vector2 Bezier(float u)
    {
        float one = 1f - u;
        return one * one * p0 + 2f * one * u * p1 + u * u * p2; // 2차 베지어
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
                Destroy(gameObject, 0.02f);
            }
            transform.position = hit.point;
            active = false;                  // ← 충돌시에만 비활성화
            Destroy(gameObject, 1);
        }
    }
}
