using UnityEngine;
using System.Collections;
using System.Linq;

public class BotController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public Vector2 stageMinMaxX = new(-8, 8);

    [Header("Attack")]
    public string attackTrigger = "Attack";
    public Vector2 attackInterval = new(0.6f, 1.0f); 
    public int attackBurstMin = 1, attackBurstMax = 2; 

    [Header("Skill")]
    public SkillManager skillManager;
    public SkillSlot[] skillPool = { SkillSlot.Q, SkillSlot.W, SkillSlot.E };
    public Vector2 skillInterval = new(1.2f, 2.0f); 

    public Weapon weapon;           
    public Transform player;

    bool gameEnd = false;
    Animator _animator;
    Rigidbody2D rb;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        if (!skillManager) skillManager = GetComponent<SkillManager>();
        if (skillManager && player) skillManager.target = player;
    }

    public void InitBot()
    {
        var w = GetComponent<Weapon>();
        w.target = PlayerController2D.inst.transform;

        var skillctrl = GetComponent<SkillManager>();
        if (skillctrl)
        {
            skillctrl.target = PlayerController2D.inst.transform;
        }

        _animator.SetTrigger("GameStart");
        var h = GetComponent<Health>();
        h.OnDeath += Die;

    }

    void OnEnable() { StartCoroutine(Loop()); }

    void Die()
    {
        _animator.SetTrigger("Die");
        gameEnd = true;
        GameManager.inst.GameEnd();
    }

    IEnumerator Loop()
    {
        var waitShort = new WaitForSeconds(0.1f);
        while (true)
        {
            // 1) 이동
            float dir = Random.value < 0.5f ? -1f : 1f;
            float dur = Random.Range(.3f, 1.0f);
            _animator.SetBool("IsMove", true);
            float t = 0f;
            while (t < dur)
            {
                if (gameEnd)
                    break;
                t += Time.deltaTime;
                rb.velocity = new Vector2(dir * moveSpeed, 0);
                Clamp();
                yield return null;
            }

            rb.velocity = Vector2.zero;
            _animator.SetBool("IsMove", false);
            if (gameEnd)
                break;

            // 2) 스킬
            if (skillManager)
            {
                // 쿨다운 없는 슬롯 후보
                var candidates = skillPool.Where(s => {
                    var so = skillManager.GetEquipped(s);
                    if (so == null) return false;
                    var st = skillManager.GetSlotState(s);
                    return st.cdRemain <= 0f && !st.casting;
                }).ToArray();

                if (candidates.Length > 0)
                {
                    var slot = candidates[Random.Range(0, candidates.Length)];
                    skillManager.TryCast(slot);
                    // 시전 중이면 잠깐 대기
                    float guard = 0.5f;
                    while (guard > 0f && skillManager.GetSlotState(slot).casting)
                    {
                        guard -= Time.deltaTime;
                        yield return null;
                    }
                }
            }
            if (gameEnd)
                break;

            // 4) 다음 사이클 전 대기
            yield return new WaitForSeconds(Random.Range(skillInterval.x, skillInterval.y));
        }
    }

    void Clamp()
    {
        var p = transform.position;
        p.x = Mathf.Clamp(p.x, stageMinMaxX.x, stageMinMaxX.y);
        transform.position = p;
    }

    public void WinMotion()
    {
        _animator.SetTrigger("Win");
        gameEnd = true;
    }
}
