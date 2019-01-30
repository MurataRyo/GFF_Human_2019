using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeMove : EnemyMove {

    public override void Attack()
    {
        if (!attackFlag && !enemy.rangeFlag)
        {
            StartCoroutine(MinRangeMove());
        }
    }

    //近距離キャラの行動
    public IEnumerator MinRangeMove()
    {
        attackFlag = true;

        //攻撃範囲に入ってから攻撃開始するまでの時間
        yield return new WaitForSeconds(enemy.startTime);
        playerState = 1;

        //攻撃開始してから当たり判定が生まれるまでの時間
        yield return new WaitForSeconds(enemy.attackModeSpeed);

        //攻撃判定の追加
        hitFlag = true;

        //攻撃モーションの残り時間
        yield return new WaitForSeconds(enemy.attackSpeed);

        //攻撃判定の削除
        hitFlag = false;

        playerState = 0;
        //攻撃してから次の行動に移るまでの時間
        yield return new WaitForSeconds(enemy.attackInterval);

        attackFlag = false;
    }
}
