using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TentouMove : EnemyMove
{
    const float BULLET_RANGE = 0.75f;    //フンコロガシとフンの間隔
    const float BULLET_HEIGHT = 0.75f;    //フンコロガシとフンの間隔
    GameObject bulletPrefab;
    const float SPEED = 15f; 

    public override void Start()
    {
        base.Start();
        bulletPrefab = Resources.Load<GameObject>("GameObject/TentouBullet");
    }

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

        while (!AngleFlag(gameObject, playerCharacter, 0.5f))
        {
            targetAngleRotate(gameObject, playerCharacter.transform.position, rotateSpeed);
            yield return null;
        }

        //攻撃範囲に入ってから攻撃開始するまでの時間
        yield return new WaitForSeconds(enemy.startTime);

        playerState = 1;

        //弾を転がすまでの時間
        yield return new WaitForSeconds(enemy.attackModeSpeed);

        CreateBullet();

        //攻撃してからリロードに入るまでの時間
        yield return new WaitForSeconds(enemy.attackInterval);

        playerState = 0;

        attackFlag = false;
    }

    //弾の生成
    void CreateBullet()
    {
        GameObject childBullet = Instantiate(bulletPrefab);
        childBullet.transform.position = transform.position + (transform.forward * BULLET_RANGE) + Vector3.up * BULLET_HEIGHT;
        childBullet.transform.eulerAngles = transform.eulerAngles;
        childBullet.GetComponent<Rigidbody>().velocity = transform.forward * SPEED;
    }

    //Centerの場所へMoveObをSpeed(360°回る時間)の速さで向く。
    void targetAngleRotate(GameObject MoveOb, Vector3 Center, float Speed)
    {
        float angleY;
        bool ine;

        Vector3 dif = Center - MoveOb.transform.position;

        // ラジアン
        float radian = -Mathf.Atan2(dif.z, dif.x);
        float degree = (radian * Mathf.Rad2Deg);
        degree += 90f;  //degreeは向きたい角度

        ine = Mathf.DeltaAngle(MoveOb.transform.eulerAngles.y, degree) >= 0f;

        angleY = MoveOb.transform.eulerAngles.y;
        angleY += 360f / (BoolToInt(ine) * Speed) * Time.deltaTime;

        //回りすぎたときの補間
        if (BoolToInt(ine) * Mathf.DeltaAngle(angleY, degree) < 0f)
        {
            angleY = degree;
        }

        MoveOb.transform.eulerAngles = new Vector3
            (MoveOb.transform.eulerAngles.x,
            angleY,
            MoveOb.transform.eulerAngles.z);
    }

    //trueを+1 falseを-1として返す
    int BoolToInt(bool Inequality)
    {
        return (Convert.ToInt32(Inequality) * 2) - 1;
    }

    //goがセンターの方向に向いているか angleが誤差分
    bool AngleFlag(GameObject go, GameObject center, float angle)
    {
        Vector3 dif = center.transform.position - go.transform.position;
        // ラジアン
        float radian = -Mathf.Atan2(dif.z, dif.x);
        float degree = (radian * Mathf.Rad2Deg);
        degree += 90f;  //degreeはgoがどれだけ向いたら向けるか

        //角度を出す
        float angle2 = Mathf.DeltaAngle(go.transform.eulerAngles.y, degree);

        if (angle2 < angle && angle2 > -angle)
        {
            return true;
        }
        return false;
    }
}
