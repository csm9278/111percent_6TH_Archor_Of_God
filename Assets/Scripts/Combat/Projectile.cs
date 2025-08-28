using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Damage/Team")]
    public int damage = 10;
    public string ownerTeam = "Player";
    public LayerMask hitMask;
    public float hitRadius = 0.12f;

    [Header("Bezier Motion")]
    public float duration = 0.6f;   // � ���� �ð�
    public float arcHeight = 2.0f;  // ������ ��� ����(+Y)
    public bool faceVelocity = true;

    [Header("After Bezier")]
    public bool continuePastEnd = true;  // ������ ����
    public float postSpeed = 12f;        // ���� �ӵ�
    public float maxLife = 5f;           // ��������(���� ����)

    Vector2 p0, p1, p2;                  // start, control, end
    float t;                             // 0..1
    bool active;                         // �浹 �ÿ��� false
    bool bezierDone;                     // ������ ���� ���� ����
    Vector2 lastPos;
    Vector2 postDir;                     // ������ ���� ����
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

            // ���� �̵� ��� �浹 üũ
            SweepHit(lastPos, pos);
            if (!active) return;

            // ȸ�� ó��
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

            // ������ ���� �� ���� �������� ��ȯ
            if (u >= 1f)
            {
                bezierDone = true;
                Vector2 tangent = (p2 - p1); // 2�� ������ ���� ����
                if (tangent.sqrMagnitude < 1e-6f) tangent = (pos - p1);
                postDir = tangent.normalized;
            }
        }
        else
        {
            // ���� ����
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
        return one * one * p0 + 2f * one * u * p1 + u * u * p2; // 2�� ������
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
            active = false;                  // �� �浹�ÿ��� ��Ȱ��ȭ
            Destroy(gameObject, 1);
        }
    }
}
