
using System;
using UnityEngine;

public class ExterminationMode : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyHealth[] enemies = new EnemyHealth[0];

    [Header("Extermination State")]
    [SerializeField] private int remainingEnemyCount = 0;
    [SerializeField] private bool isExterminationCompleted = false;

    public event Action OnExterminationCompleted;

    public int RemainingEnemyCount => remainingEnemyCount;
    public bool IsExterminationCompleted => isExterminationCompleted;

    private void Awake()
    {
        InitializeEnemies();
    }

    private void OnDestroy()
    {
        UnsubscribeEnemyEvents();
    }

    public void InitializeEnemies()
    {
        UnsubscribeEnemyEvents();

        remainingEnemyCount = 0;
        isExterminationCompleted = false;

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
            {
                continue;
            }

            enemy.OnEnemyDied += HandleEnemyDied;
            remainingEnemyCount++;
        }

        if (remainingEnemyCount == 0)
        {
            CompleteExtermination();
        }
    }

    private void HandleEnemyDied(EnemyHealth InEnemy)
    {
        if (isExterminationCompleted || InEnemy == null)
        {
            return;
        }

        InEnemy.OnEnemyDied -= HandleEnemyDied;

        remainingEnemyCount--;

        if (remainingEnemyCount <= 0)
        {
            CompleteExtermination();
        }
    }

    private void CompleteExtermination()
    {
        if (isExterminationCompleted)
        {
            return;
        }

        isExterminationCompleted = true;
        remainingEnemyCount = 0;

        OnExterminationCompleted?.Invoke();

        Debug.Log("Extermination completed!");
    }

    private void UnsubscribeEnemyEvents()
    {
        if (enemies == null)
        {
            return;
        }

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied -= HandleEnemyDied;
            }
        }
    }
}
