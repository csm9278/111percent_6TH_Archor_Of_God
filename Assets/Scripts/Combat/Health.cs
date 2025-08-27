using UnityEngine;
public class Health : MonoBehaviour
{
    public int maxHP = 100;
    public int Current { get; private set; }
    public System.Action OnDeath, OnDamaged;
    void Awake() { Current = maxHP; }
    public void Heal(int v) { Current = Mathf.Min(maxHP, Current + v); }
    public void Damage(int v)
    {
        if (Current <= 0) return;
        Current -= v;
        OnDamaged?.Invoke();
        if (Current <= 0) OnDeath?.Invoke();
    }
}
