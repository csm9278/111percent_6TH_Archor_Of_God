using UnityEngine;
public class Damageable : MonoBehaviour
{
    public string team = "Player";
    Health hp; void Awake() => hp = GetComponent<Health>();
    public void ApplyDamage(int dmg) { hp.Damage(dmg); }
}
