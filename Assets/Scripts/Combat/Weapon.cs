using UnityEngine;
public class Weapon : MonoBehaviour
{
    public Projectile projPrefab; public Transform muzzle;
    public string team; public Transform target;

    void Shoot()
    {
        var p = Instantiate(projPrefab, muzzle.position, Quaternion.identity);
        p.ownerTeam = team;
        p.hitMask = LayerMask.GetMask("Default", "Enemy"); // 필요에 맞게
        p.Launch(muzzle.position, target.position); // 시간, 호 높이
    }
}
