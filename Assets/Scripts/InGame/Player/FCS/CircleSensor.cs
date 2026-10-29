using System.Collections.Generic;
using UnityEngine;

public class CircleSensor : MonoBehaviour
{
    public List<GameObject> enemyList = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (!enemyList.Contains(collision.gameObject))
            {
                enemyList.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (enemyList.Contains(collision.gameObject))
            {
                enemyList.Remove(collision.gameObject);
            }
        }
    }
}
