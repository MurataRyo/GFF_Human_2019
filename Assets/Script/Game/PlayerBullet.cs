using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] float BULLET_DESTROY;

    void Start()
    {
        Destroy(this.gameObject, BULLET_DESTROY);  // 発射して少し経ったら消す
    }

    // 何かに当たった時の処理
    void OnTriggerStay(Collider col)
    {
        if (col.isTrigger)
        {
            return;
        }

        if (col.gameObject.tag == "Enemy")
        {
            EnemyMove enemyMove = col.gameObject.GetComponent<EnemyMove>();

            if (enemyMove.life <= 1)
            {
                enemyMove.DestoryEnemy();
            }
            else
            {
                enemyMove.life--;
            }
            Destroy(gameObject);
        }

    }
}
