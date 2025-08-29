using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D inst;
    public float moveSpeed = 6f;
    public Vector2 stageMinMaxX = new(-8f, 8f);
    Rigidbody2D rb; float input;
    Animator _animator;
    public bool skillCasting = false;
    bool isDie = false;
    public SkillUI[] skillUis;
    SkillManager skillManager;
    public void InitPlayer(GameObject enemy)
    {
        var w = GetComponent<Weapon>();
        w.target = enemy.transform;

        var skillctrl = GetComponent<SkillManager>();
        if(skillctrl)
        {
            skillctrl.target = enemy.transform;
            Debug.Log(skillctrl.GetCtx());
        }

        _animator.SetTrigger("GameStart");

    }

    void Awake()
    {
        inst = this;
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        skillManager = GetComponent<SkillManager>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var h = GetComponent<Health>();
        h.OnDeath += Die;
    }

    void Update()
    {
        if (!GameManager.inst.gameStart)
            return;

        if (isDie)
            return;

        if (skillCasting)
        {
            if (input != 0)
                input = 0;
            return;
        }

        input = Input.GetAxisRaw("Horizontal"); // -1,0,1
        if (input != 0)
            _animator.SetBool("IsMove", true);
        else
            _animator.SetBool("IsMove", false);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        Vector2 p = rb.position;

        // 경계에서 바깥으로 나가려는 입력 무시
        if ((p.x <= stageMinMaxX.x && input < 0f) ||
            (p.x >= stageMinMaxX.y && input > 0f)) input = 0f;

        float nextX = Mathf.Clamp(p.x + input * moveSpeed * dt, stageMinMaxX.x, stageMinMaxX.y);
        rb.MovePosition(new Vector2(nextX, p.y)); // velocity 사용 안 함
    }

    void Die()
    {
        _animator.SetTrigger("Die");
        isDie = true;
        GameManager.inst.GameEnd(true);
    }

    public void ResetPlayer()
    {
        this.transform.position = new Vector3(-8.5f, -1.96f, -5f);
        _animator.SetTrigger("Reset");
        var health = GetComponent<Health>();
        if(health)
            health.ResetHealth();
        isDie = false;
    }

    public void RefreshSkillUI()
    {

        for (int i = 0; i < 3; i++)
        {
            //Debug.Log(skillManager.GetEquipped((SkillSlot)i).id);
            skillUis[i].SetSkill(skillManager.GetEquipped((SkillSlot)i));
        }
    }

    public void WinMotion()
    {
        _animator.SetTrigger("Win");
    }
}
