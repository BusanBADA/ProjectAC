using System;
using System.Collections.Generic;
using UnityEngine;

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
        Vector2 playerInput = playerRigid != null ? playerRigid.linearVelocity : Vector2.zero;

        if (enemyInTriangle.Count == 0 && enemyInCircle.Count == 0)
        {
            if (playerInput.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(playerInput.y, playerInput.x) * Mathf.Rad2Deg - 90f;

                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                if (upperBodyTrs != null) upperBodyTrs.rotation = transform.rotation;//Mouse
            }
        }
        else
        {
            float angle = Mathf.Atan2(nearestVec.y, nearestVec.x) * Mathf.Rad2Deg - 90f;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            if (upperBodyTrs != null) upperBodyTrs.rotation = transform.rotation;
        }

    }

    private void EnemiesInit()//mode에서 받아온 enemy를 struct로 만듦.
    {
        if (eliminationMode != null)
        {
            IReadOnlyList<MechHull2D> enemyIReadOnlyList = eliminationMode.Enemies;

            if (enemies == null || enemies.Length != enemyIReadOnlyList.Count)
            {
                enemies = new EnemyData[enemyIReadOnlyList.Count];
            }

            Vector2 currentPos = transform.position;

            for (int i = 0; i < enemyIReadOnlyList.Count; i++)
            {
                MechHull2D enemyHull = enemyIReadOnlyList[i];

                // [수정] 이미 사망했거나 null인 적은 제외 처리 (LateUpdate 전 딜레이 방지)
                if (enemyHull != null && enemyHull.gameObject != null && !enemyHull.IsDead)
                {
                    Vector2 enemyPos = enemyHull.transform.position;// 매 프레임 위치 기반으로 방향과 거리를 갱신.

                    enemies[i] = new EnemyData//구조체 도중의 코드는 못 사용함.
                    {
                        enemyObj = enemyHull.gameObject,
                        direction = (enemyPos - currentPos).normalized,
                        distance = Vector2.Distance(currentPos, enemyPos)
                    };
                }
                else
                {
                    enemies[i] = new EnemyData { enemyObj = null }; // 파괴되거나 죽은 적 처리
                }
            }
        }
    }

    private void EnemyInRangeInit() // 범위 내에 들어온 적들을 리스트에 넣음.
    {
        if (enemies != null)
        {
            foreach (EnemyData enemy in enemies)
            {
                if (enemy.enemyObj == null) continue;

                //삼각형 센서 처리
                bool isInTriangleList = false;
                foreach (EnemyData inTriangle in enemyInTriangle)
                {
                    if (inTriangle.enemyObj == enemy.enemyObj)
                    {
                        isInTriangleList = true;
                        break;
                    }
                }

                if (triangleSensor != null && triangleSensor.enemyList != null && triangleSensor.enemyList.Contains(enemy.enemyObj))
                {
                    if (!isInTriangleList)
                    {
                        enemyInTriangle.Add(enemy);
                    }
                }
                else
                {
                    if (isInTriangleList)
                    {
                        // 오브젝트 기준으로 리스트에서 제거
                        enemyInTriangle.RemoveAll(inTriangle => inTriangle.enemyObj == enemy.enemyObj);
                    }
                }


                //원형 센서 처리
                bool isInCircleList = false;
                foreach (EnemyData inCircle in enemyInCircle)
                {
                    if (inCircle.enemyObj == enemy.enemyObj)
                    {
                        isInCircleList = true;
                        break;
                    }
                }

                if (circleSensor != null && circleSensor.enemyList != null && circleSensor.enemyList.Contains(enemy.enemyObj))
                {
                    if (!isInCircleList)
                    {
                        enemyInCircle.Add(enemy);
                    }
                }
                else
                {
                    if (isInCircleList)
                    {
                        // 오브젝트 기준으로 리스트에서 제거
                        enemyInCircle.RemoveAll(inCircle => inCircle.enemyObj == enemy.enemyObj);
                    }
                }
            }
        }
    }

    private void FindNearest()
    {
        nearestDistance = Mathf.Infinity;
        nearestVec = Vector2.zero; // [수정] 타겟이 없을 때 이전 방향이 남지 않도록 초기화

        enemyInTriangle.RemoveAll(e => e.enemyObj == null);
        enemyInCircle.RemoveAll(e => e.enemyObj == null);

        UpdateListData(enemyInTriangle);
        UpdateListData(enemyInCircle);

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
        else if (enemyInCircle.Count > 0) // [수정] 안전하게 Count가 0보다 클 때만 순회
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

    private void UpdateListData(List<EnemyData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            // enemies 배열에서 해당 적의 최신 데이터를 찾아 리스트를 갱신
            for (int j = 0; j < enemies.Length; j++)
            {
                if (enemies[j].enemyObj == list[i].enemyObj)
                {
                    list[i] = enemies[j];
                    break;
                }
            }
        }
    }
}