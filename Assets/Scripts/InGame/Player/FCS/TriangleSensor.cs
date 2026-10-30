using System.Collections.Generic;
using UnityEngine;

public class TriangleSensor : MonoBehaviour
{
    public List<GameObject> enemyList = new List<GameObject>();

    private void Update()
    {
        // 적이 도중에 파괴(Destroy)되었을 경우 리스트에서 안전하게 제거
        enemyList.RemoveAll(enemy => enemy == null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Armor"))
        {
            if (!enemyList.Contains(collision.gameObject))
            {
                enemyList.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Armor"))
        {
            if (enemyList.Contains(collision.gameObject))
            {
                enemyList.Remove(collision.gameObject);
            }
        }
    }

}