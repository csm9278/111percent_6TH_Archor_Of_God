using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 6f; public Vector2 stageMinMaxX = new(-8f, 8f);
    Rigidbody2D rb; float input;
    void Awake() { rb = GetComponent<Rigidbody2D>(); rb.gravityScale = 0; }
    void Update() { input = Input.GetAxisRaw("Horizontal"); }
    void FixedUpdate()
    {
        Vector2 v = new(input * moveSpeed, 0);
        rb.velocity = v;
        var p = transform.position;
        p.x = Mathf.Clamp(p.x, stageMinMaxX.x, stageMinMaxX.y);
        transform.position = p;
    }
}
