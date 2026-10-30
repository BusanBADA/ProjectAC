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

    [Header("EliminationMode")]
    [SerializeField]
    private EliminationMode eliminationMode;
    private EnemyData[] enemies;//data

    [Header("Target")]
    private float nearestDistance = Mathf.Infinity;
    private Vector2 nearestVec;

    [Header("Sensor")]
    [SerializeField]
    private TriangleSensor triangleSensor;
    [SerializeField]
    private CircleSensor circleSensor;
    List<EnemyData> enemyInTriangle;//data
    List<EnemyData> enemyInCircle;//data

    private void Awake()
    {
        enemyInTriangle = new List<EnemyData>();
        enemyInCircle = new List<EnemyData>();
    }

    private void Update()
    {
        EnemiesInit();
        EnemyInRangeInit();
        FindNearest();
        RotateToRange();
    }

    private void RotateToRange()//범위를 진행방향에 따라 회전시킴.
    {
        if(enemyInTriangle.Count == 0 && enemyInCircle.Count == 0)
        {
            Vector2 playerInput = playerRigid.linearVelocity;

            float angle = Mathf.Atan2(playerInput.y, playerInput.x) * Mathf.Rad2Deg;

            angle -= 90f;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            upperBodyTrs.rotation = transform.rotation;//Mouse
        }
        else
        {
            float angle = Mathf.Atan2(nearestVec.y, nearestVec.x) * Mathf.Rad2Deg;
            angle -= 90f;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            upperBodyTrs.rotation = transform.rotation; 
        }
        
    }

    private void EnemiesInit()//mode에서 받아온 enemy를 struct로 만듦.
    {
        if (eliminationMode != null)
        {
            IReadOnlyList<MechHull2D> enemyIReadOnlyList = eliminationMode.Enemies;

            enemies = new EnemyData[enemyIReadOnlyList.Count];

            for (int i = 0; i < enemyIReadOnlyList.Count; i++)
            {
                MechHull2D enemyHull = enemyIReadOnlyList[i];

                if (enemyHull != null)
                {
                    enemies[i] = new EnemyData//구조체 도중의 코드는 못 사용함.
                    {
                        enemyObj = enemyHull.gameObject,
                        direction = (enemyHull.gameObject.transform.position - transform.position).normalized,
                        distance = Vector2.Distance(transform.position, enemyHull.gameObject.transform.position)
                    };
                }
            }
        }
    }

    private void EnemyInRangeInit()//범위 내에 들어온 적들을 리스트에 넣음.
    {
        if (enemies != null)
        {
            foreach (EnemyData enemy in enemies)
            {
                // 1. 삼각형 센서 처리
                if (triangleSensor.enemyList.Contains(enemy.enemyObj))
                {
                    // 이미 리스트에 없을 때만 추가 (중복 방지)
                    if (!enemyInTriangle.Contains(enemy))
                    {
                        enemyInTriangle.Add(enemy);
                    }
                }
                else
                {
                    // 센서 밖으로 나갔고 리스트에 들어있다면 제거
                    if (enemyInTriangle.Contains(enemy))
                    {
                        enemyInTriangle.Remove(enemy);
                    }
                }

                // 2. 원형 센서 처리
                if (circleSensor.enemyList.Contains(enemy.enemyObj))
                {
                    if (!enemyInCircle.Contains(enemy))
                    {
                        enemyInCircle.Add(enemy);
                    }
                }
                else
                {
                    if (enemyInCircle.Contains(enemy))
                    {
                        enemyInCircle.Remove(enemy);
                    }
                }
            }
        }
    }

    private void FindNearest()
    {
        nearestDistance = Mathf.Infinity;

        if (enemyInTriangle.Count > 0)
        {
            foreach (EnemyData enemy in enemyInTriangle)
            {
                if (enemy.distance < nearestDistance)
                {
                    nearestDistance = enemy.distance;
                    nearestVec = enemy.direction;
                }
            }
        }
        else if(enemyInTriangle.Count == 0)
        {
            foreach (EnemyData enemy in enemyInCircle)
            {
                if (enemy.distance < nearestDistance)
                {
                    nearestDistance = enemy.distance;
                    nearestVec = enemy.direction;
                }
            }
        }
    }

}

//EliminationMode의 적을 받아서 FCS에 저장한다. TriangleSensor와 CircleSensor에서도 리스트를 받아서 FCS에 저장한다.
//범위 내의 적을 리스트에 담는다. 범위 밖으로 나간 적은 리스트에서 뺀다.
//담은 적 거리의 우선순위를 매긴다. 1순위 삼각형, 2순위 원.
//1순위에 해당하는 적을 쳐다본다.
//리스트는 항상 업데이트되어야한다.