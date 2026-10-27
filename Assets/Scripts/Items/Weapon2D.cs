
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

    [Header("Test Rotation")]
    [SerializeField] private bool enableTestRotation = true;
    [SerializeField, Min(0f)] private float rotationSpeed = 90f;

    private float nextFireTime;

    private void Update()
    {
        if (autoFire)
        {
            TryFire();
        }
    }

    private void TryFire()
    {
        if (projectilePrefab == null || muzzle == null)
            return;

        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireInterval;

        Instantiate(projectilePrefab, muzzle.position,muzzle.rotation);
    }
}
