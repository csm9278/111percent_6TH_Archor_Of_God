// BurnDoT.cs
using UnityEngine;
using System.Collections;

public class BurnDoT : MonoBehaviour
{
    Coroutine co;
    public void Apply(float duration, float tick, int dmgPerTick)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Co(duration, tick, dmgPerTick));
    }
    IEnumerator Co(float duration, float tick, int dmg)
    {
        float t = 0f;
        var hp = GetComponent<Health>();
        // ����: �ߺ� ���� ��å(�����). ���� ���ϸ� �ջ� ���� �߰�.
        while (t < duration && hp)
        {
            hp.Damage(dmg);
            Debug.Log(hp.Current);
            yield return new WaitForSeconds(tick);
            t += tick;
        }
        co = null;
    }
}
