using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Health State")]
    public bool IsDead => isDead;
    public event Action<EnemyHealth> OnEnemyDied;

    private int currentHealth = 0;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int InDamage)
    {
        if (isDead || InDamage <= 0)
        {
            return;
        }

        currentHealth -= InDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        OnEnemyDied?.Invoke(this);

        Destroy(gameObject);
    }
}
