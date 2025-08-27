using UnityEngine;
public class Weapon : MonoBehaviour
{
    public Projectile projPrefab; public Transform muzzle;
    public string team; public Transform target;

    void Shoot()
    {
        var p = Instantiate(projPrefab, muzzle.position, Quaternion.identity);
        p.ownerTeam = team;
        p.hitMask = LayerMask.GetMask("Default", "Enemy"); // �ʿ信 �°�
        p.Launch(muzzle.position, target.position); // �ð�, ȣ ����
    }
}
