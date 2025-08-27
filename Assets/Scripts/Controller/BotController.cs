using UnityEngine;
public class BotController : MonoBehaviour
{
    public float moveSpeed = 5f; public Vector2 stageMinMaxX = new(-8, 8);
    public Weapon weapon;/* public SkillController skills;*/ public Transform player;
    Animator _animator;

    Rigidbody2D rb; 
    void Awake() 
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); 
        rb.gravityScale = 0; 
    }
    void OnEnable() 
    {
        StartCoroutine(Loop()); 
    }

    System.Collections.IEnumerator Loop()
    {
        while (true)
        {
            // Move
            float dir = Random.value < 0.5f ? -1f : 1f;
            float dur = Random.Range(.3f, 1.0f);
            _animator.SetBool("IsMove", true);
            float t = 0; while (t < dur)
            {
                t += Time.deltaTime; rb.velocity = new Vector2(dir * moveSpeed, 0);
                Clamp(); yield return null;
            }
            _animator.SetBool("IsMove", false);
            rb.velocity = Vector2.zero;
            // StopAttack
            dur = Random.Range(0.8f, 1.2f);
            t = 0; while (t < dur) { t += Time.deltaTime; yield return null; }
            // Skill
            //int pick = Random.Range(0, 3);
            //if (pick == 0) GetComponent<SkillDash>()?.TryCast();
            //else if (pick == 1) GetComponent<SkillVolley>()?.TryCast();
            //else GetComponent<SkillShield>()?.TryCast();
        }
    }
    void Clamp()
    {
        var p = transform.position; p.x = Mathf.Clamp(p.x, stageMinMaxX.x, stageMinMaxX.y); transform.position = p;
    }
}
