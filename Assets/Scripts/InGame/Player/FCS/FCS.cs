using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct EnemyData
{
    public GameObject enemyObj;
    public Vector2 direction;
    public float distance;
}

public class FCS : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Rigidbody2D playerRigid;
    [SerializeField]
    private Transform upperBodyTrs;

    [Header("FromEliminationMode")]
    [SerializeField] 
    private EliminationMode eliminationMode;
    private EnemyData[] enemies;

    [Header("Target")]
    [SerializeField]
    private List<EnemyData> targets = new List<EnemyData>();
    private float nearestDistance = Mathf.Infinity;

    [Header("Sensor")]
    [SerializeField] private PolygonCollider2D triangleSensor;
    [SerializeField] private CircleCollider2D circleSensor;

    private void Start()
    {

    }
    private void Update()
    {
        RotateToRange();
        EnemiesToStructArray();
    }

    private void RotateToRange()
    {
        Vector2 playerInput = playerRigid.linearVelocity;

        float angle = Mathf.Atan2(playerInput.y, playerInput.x) * Mathf.Rad2Deg;

        angle -= 90f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        if (playerInput.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            upperBodyTrs.rotation = transform.rotation;
        }
    }

    private void EnemiesToStructArray()
    {
        if (eliminationMode != null)
        {
            IReadOnlyList<MechHull2D> enemyIReadOnlyList = eliminationMode.Enemies;

            enemies = new EnemyData[enemyIReadOnlyList.Count];

            for (int i =0; i < enemyIReadOnlyList.Count; i++)
            {
                MechHull2D enemyHull = enemyIReadOnlyList[i];

                if (enemyHull != null)
                {
                    enemies[i] = new EnemyData
                    {
                        enemyObj = enemyHull.gameObject,
                        direction = (enemies[i].enemyObj.transform.position - transform.position).normalized,
                        distance = Vector2.Distance(transform.position, enemies[i].enemyObj.transform.position)
                    };
                }
            }
        }
    }

    private void UpdateList()
    {

    }

    private void LookNearEnemy()
    {
        if(targets.Count == 0)
        {
            return;
        }

        foreach(EnemyData enemyData in enemies)
        {
            if (enemyData.distance < nearestDistance)
            {
                nearestDistance = enemyData.distance;
            }
        }
        //EliminationMode의 적을 받아서 FCS에 저장한다. TriangleSensor와 CircleSensor에서도 리스트를 받아서 FCS에 저장한다.
        //
        //범위 내의 적을 리스트에 담는다. 범위 밖으로 나간 적은 리스트에서 뺀다.
        //담은 적 거리의 우선순위를 매긴다. 1순위 부채꼴, 2순위 원.
        //1순위에 해당하는 적을 쳐다본다.
        //우선순위는 꾸준히 업데이트된다.
    }

}