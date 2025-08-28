using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum PathMode { Bezier, Linear }          // ★ 추가

    [Header("Damage/Team")]
    public int damage = 10;
    public string ownerTeam = "Player";
    public LayerMask hitMask;
    public float hitRadius = 0.12f;

    [Header("Path")]
    public PathMode pathMode = PathMode.Bezier;      // ★ 추가

    [Header("Bezier Motion")]
    public float duration = 0.6f;
    public float arcHeight = 2.0f;
    public bool faceVelocity = true;

    [Header("After Bezier")]
    public bool continuePastEnd = true;
    public float postSpeed = 12f;
    public float maxLife = 5f;

    [Header("Linear Motion")]                        // ★ 추가
    public float linearSpeed = 14f;                  // 직선 속도
    public bool stopAtEndPoint = true;               // 목표점 도달 시 종료

    Vector2 p0, p1, p2;
    float t, life;
    bool active, bezierDone;
    Vector2 lastPos, postDir;

    // Linear용
    Vector2 linearDir;                                // ★ 추가

    public void Launch(Vector2 start, Vector2 end, float durationSec, float arc)
    {
        p0 = start; p2 = end;
        transform.position = p0; lastPos = p0;

        life = 0f; active = true;

        if (pathMode == PathMode.Bezier)
        {
            duration = Mathf.Max(0.01f, durationSec);
            arcHeight = arc;
            Vector2 mid = (p0 + p2) * 0.5f;
            p1 = mid + Vector2.up * arcHeight;
            t = 0f; bezierDone = false; postDir = Vector2.zero;
        }
        else // Linear
        {
            linearDir = (p2 - p0).sqrMagnitude > 1e-8f ? (p2 - p0).normalized : Vector2.right;
        }
    }

    void Update()
    {
        if (!active) return;

        float dt = Time.deltaTime;
        life += dt; if (life >= maxLife) { Destroy(gameObject); return; }

        if (pathMode == PathMode.Bezier) UpdateBezier(dt);
        else UpdateLinear(dt);
    }

    void UpdateLinear(float dt)                               // ★ 추가
    {
        Vector2 next = (Vector2)transform.position + linearDir * linearSpeed * dt;

        // 목표점을 지나쳤는지 체크(선분 종단 처리)
        if (stopAtEndPoint)
        {
            Vector2 toEndPrev = p2 - (Vector2)transform.position;
            Vector2 toEndNext = p2 - next;
            if (Vector2.Dot(toEndPrev, toEndNext) <= 0f)
            {
                // 목표점까지 스냅
                SweepHit(lastPos, p2);
                if (!active) return;
                transform.position = p2;
                lastPos = p2;
                active = false;
                Destroy(gameObject, 0.6f);
                return;
            }
        }

        SweepHit(lastPos, next);
        if (!active) return;

        FaceAndMove(next);
    }

    void UpdateBezier(float dt)
    {
        Vector2 pos;

        if (!bezierDone)
        {
            t += dt / duration;
            float u = Mathf.Clamp01(t);
            pos = Bezier(u);

            SweepHit(lastPos, pos);
            if (!active) return;

            FaceAndMove(pos);

            if (u >= 1f)
            {
                bezierDone = true;
                Vector2 tangent = (p2 - p1);
                if (tangent.sqrMagnitude < 1e-6f) tangent = (pos - p1);
                postDir = tangent.normalized;
            }
        }
        else
        {
            pos = (Vector2)transform.position + postDir * postSpeed * dt;
            SweepHit(lastPos, pos);
            if (!active) return;
            FaceAndMove(pos);
        }
    }

    void FaceAndMove(Vector2 pos)
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

    Vector2 Bezier(float u)
    {
        float one = 1f - u;
        return one * one * p0 + 2f * one * u * p1 + u * u * p2;
    }

    void SweepHit(Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from; float dist = dir.magnitude;
        if (dist <= 1e-6f) return;

        var hit = Physics2D.CircleCast(from, hitRadius, dir.normalized, dist, hitMask);
        if (hit.collider != null)
        {
            var d = hit.collider.GetComponentInParent<Damageable>(); // 더 견고하게
            if (d != null && d.team != ownerTeam)
            {
                d.ApplyDamage(damage);
                Destroy(gameObject, 0.02f);
            }
            transform.position = hit.point;
            active = false;
            Destroy(gameObject, 1f);
        }
    }
}
