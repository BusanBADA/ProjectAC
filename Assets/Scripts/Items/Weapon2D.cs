
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon2D : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;

    [Header("Auto Fire")]
    [SerializeField] private bool autoFire = true;
    [SerializeField, Min(0.01f)] private float fireInterval = 1.0f;
    [SerializeField]
    private ProjectileOwner projectileOwner = ProjectileOwner.Player;
    private bool canFire;

    private float nextFireTime;

    private void Update()
    {
        if (autoFire && canFire)
        {
            TryFire();
        }

    }
    public void SetCanFire(bool value)
    {
        canFire = value;
    }

    private void TryFire()
    {
        if (projectilePrefab == null || muzzle == null)
            return;

        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireInterval;

        GameObject projectile = Instantiate(
        projectilePrefab,
        muzzle.position,
        muzzle.rotation
        );

        Projectile2D projectileScript = projectile.GetComponent<Projectile2D>();
        projectileScript.SetOwner(projectileOwner);
    }
}
