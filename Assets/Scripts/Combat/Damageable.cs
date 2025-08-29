using UnityEngine;
public enum DamageType
{
    Normal,
    Fire,
}

public class Damageable : MonoBehaviour
{
    public GameObject damageTextPrefab; 
    public Canvas uiCanvas;
    public string team = "Player"; 
    Health hp; void Awake() => hp = GetComponent<Health>();
    public void ApplyDamage(int dmg, DamageType dmgType = DamageType.Normal) 
    {
        if (dmg <= 0)
            return;
        string dmgString = "";

        switch(dmgType)
        {
            case DamageType.Fire:
                dmgString += "È­»ó ";
                break;
        }

        dmgString += dmg.ToString();

        hp.Damage(dmg);
        if (damageTextPrefab && uiCanvas)
        {
            var go = Instantiate(damageTextPrefab, uiCanvas.transform, false);
            var ui = go.GetComponent<DamageText>();
            ui.Init(dmgString, transform.position, uiCanvas, Camera.main);
        }
    }
}
