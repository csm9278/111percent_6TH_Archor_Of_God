using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D inst;
    public float moveSpeed = 6f;
    public Vector2 stageMinMaxX = new(-8f, 8f);
    Rigidbody2D rb; float input;
    Animator _animator;

    void Awake()
    {
        inst = this;
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
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
}
