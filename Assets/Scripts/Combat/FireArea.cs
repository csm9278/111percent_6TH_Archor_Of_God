// FireArea.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireArea : MonoBehaviour
{
    public string ownerTeam = "Player";
    public float radius = 1.5f;
    public float duration = 4f;
    public float tick = 0.5f;
    public int damagePerTick = 2;
    public LayerMask hitMask; // Player/Enemy만

    CircleCollider2D col;

    public void Init(string ownerTeam, float radius, float duration, float tick, int dmgPerTick)
    {
        this.ownerTeam = ownerTeam;
        this.radius = radius;
        this.duration = duration;
        this.tick = tick;
        this.damagePerTick = dmgPerTick;
        hitMask = LayerMask.GetMask("Player", "Enemy");
    }

    public void Init(string ownerTeam)
    {
        this.ownerTeam = ownerTeam;
    }
    void Awake()
    {
        col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    void OnEnable()
    {
        col.radius = radius;
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        float t = 0f;
        while (t < duration)
        {
            // 영역 내 대상 샘플링
            var hits = Physics2D.OverlapCircleAll(transform.position, radius, hitMask);
            foreach (var h in hits)
            {
                var d = h.GetComponentInParent<Damageable>();
                if (d != null && d.team != ownerTeam)
                    d.ApplyDamage(damagePerTick, DamageType.Fire);
            }
            yield return new WaitForSeconds(tick);
            t += tick;
        }
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
