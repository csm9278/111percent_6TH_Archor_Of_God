// AttackSpeedBuffRuntime.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class AttackSpeedBuffRuntime : MonoBehaviour
{
    Animator anim;
    float baseSpeed = 1f;
    readonly List<float> active = new();

    void Awake() { anim = GetComponent<Animator>(); baseSpeed = anim ? anim.speed : 1f; }
    void OnDisable() { if (anim) anim.speed = baseSpeed; active.Clear(); }

    public void Apply(float mult, float duration)
    {
        if (!anim) return;
        active.Add(mult);
        Recompute();
        StartCoroutine(RemoveLater(mult, duration));
    }

    IEnumerator RemoveLater(float mult, float dur)
    {
        yield return new WaitForSeconds(dur);
        active.Remove(mult);
        Recompute();
    }

    void Recompute()
    {
        float m = active.Count > 0 ? active.Max() : 1f;
        anim.speed = baseSpeed * m;
    }
}
