using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHP = 100;
    public int Current { get; private set; }
    public Action OnDeath, OnDamaged;
    public Action<int, int> OnChanged; // ¡ç Ãß°¡ (current, max)

    void Awake() { Current = maxHP; OnChanged?.Invoke(Current, maxHP); }

    public void Heal(int v)
    {
        if (Current <= 0) return;
        Current = Mathf.Min(maxHP, Current + v);
        OnChanged?.Invoke(Current, maxHP);
    }
    public void Damage(int v)
    {
        if (Current <= 0) return;
        Current -= v;
        OnDamaged?.Invoke();
        OnChanged?.Invoke(Current, maxHP);
        if (Current <= 0) OnDeath?.Invoke();
    }
}
