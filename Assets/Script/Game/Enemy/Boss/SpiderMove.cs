using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpiderMove : MonoBehaviour {
    GameObject player;  //キャラクター
    [SerializeField]
    GameObject bugBlock;//バグの親
    Rigidbody rBody;

    //攻撃の種類
    enum AttackMode
    {
        net,    //糸でプレイヤーの足止め
        bug,    //蜘蛛の糸でバグを投げる
        blow,   //体当たり
    }
    AttackMode attackMode;
    int attackNum = 3;  //Attackモードの種類

    //移動関連----------------------------------------------------------------
    bool isGround = true;       //設置判定
    const float ROTATE_SPEED = 10f;     //1回転する時間
    const float FORWARD_SPEED = 2f;     //前進、後退する速さ
    const float FORWARD_RANGE = 12f;    //前進する距離(それ以上は近寄らない)
    const float BACK_RANGE = 11f;       //下がる距離（それより近ければ後退する）
    const float RIGHT_SPEED = 5f;       //左右のスピード
    bool rightFlag = true;              //左右どちらに進んでいるかのフラグ
    const float CHANGE_RIGHT = 2f;      //左右の回転の変更する頻度
    float gravity = 0f;                 //現在の重力
    const float GRAVITY_SIZE = -9.81f;  //重力の大きさ
    Vector3 velosity = Vector3.zero;    //移動方向


    const float JUMP_TIME = 1f;
    bool jumpFlag = false;
    float jumpTimer = 0f;
    const float JUMP_POWER = 7.5f;  //ジャンプパワー
    const float BULEET_RANGE = 5f;  //弾を見つける距離
    //攻撃関連---------------------------------------------------------------
    bool attackFlag = false;        //攻撃中かどうか
    const float ATTACK_INTERVAL = 5f;   //攻撃の間隔

    //BUG攻撃関連---------------------
    [SerializeField]
    GameObject netPrefab;
    [SerializeField]
    GameObject bugBulletPrefab; //Bugの弾
    const float BUG_HAVE_RANGE = 15f;    //BUGをとる距離
    const float BUG_HAVE_MIN_RANGE = 10f;//BUGをとる距離
    const float ROTATE_BUG = 3f;        //向く速度
    const float FORWARD_BUG_SPEED = 5f; //前進後退のスピード
    const float RIGHT_BUG_SPEED = 2f;   //左右移動のスピード
    bool bugFlag = false;               //糸を吐いているかどうか
    [HideInInspector]
        public bool BugBlockFlag = false;   //ブロックを持っているかどうか
    const float BUG_SPEED = 6f;         //Bugの弾の速さ
    int bulletNum = 5; //Bugの弾の数
    const float BUG_ANGLE = 45f;    //BUGの弾の拡散角度

    //糸攻撃関連-------------------------------------------------------------
    const float NET_RANGE = 15f;    //ネットを打つ距離
    const float NET_MIN_RANGE = 10f;//ネットを打つ距離
    const float ROTATE_NET = 1.5f;    //向く速度
    [SerializeField]
    GameObject trapNetPrefab;   //糸

    //体当たり関連------------------------------------------------------------
    bool rotateBlow = false;            //回転か体当たりかどうか(falseが回転trueが体当たり)
    const float ROTATE_BLOW = 1.5f;     //向く速度
    const float BLOW_TIME = 3f;         //体当たりする時間
    const float BLOW_SPEED = 15f;       //体当たりのスピード
    const float BLOW_RANGE = 15f;       //体当たりに移る最低距離

    //共通--------------------------
    [SerializeField]
    GameObject netPos;  //糸の先端
    const float ANGLE_SAFE = 2f;        //角度を求めるときの妥協点
    float timer = 0f;


    void Start () {
        rBody = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(RightLeft());
    }
	
	// Update is called once per frame
	void Update () {
        IsGround();
        GravityControl();
        velosity = Vector3.zero;
        //攻撃のタイミング
        if (isGround && !attackFlag && TimerCount(ATTACK_INTERVAL,ref timer))
        {
            //初期化
            attackFlag = true;
            //intをEnum型へランダムに変更。
            AttackModeChange();
        }
        if(jumpFlag &&
            TimerCount(JUMP_TIME,ref jumpTimer))
        {
            jumpFlag = false;
        }

        if(Input.GetKeyDown(KeyCode.L) && !jumpFlag && isGround)
        {
            jumpFlag = true;
            gravity = JUMP_POWER;
        }

        //攻撃中
        if(attackFlag)
        {
            //モードに応じた攻撃の処理
            Attack();
        }
        //非攻撃時の行動
        else
        {
            //弾が正面にあるとジャンプする
            if(Physics.BoxCast(netPos.transform.position,new Vector3(1f,3f,0.5f),transform.forward, Quaternion.identity, BULEET_RANGE, LayerMask.GetMask("Bullet")))
            {
                gravity = JUMP_POWER;
            }
            PositonMove(player,BACK_RANGE,FORWARD_RANGE,ROTATE_SPEED, FORWARD_SPEED, RIGHT_SPEED);
        }
        //重力反映
        rBody.velocity = velosity + new Vector3(0f,gravity,0f);

    }

    //時間関係
    bool TimerCount(float time,ref float timer)
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            timer = 0f;
            return true;
        }
        return false;
    }
    
    //場所移動
    void PositonMove(GameObject movePos,float minRange,float maxRange,float rotateSpeed,float forwardSpeed,float rightSpeed)
    {
        //プレイヤーのほうを向く
        targetAngleRotate(gameObject, movePos.transform.position, rotateSpeed);

        //前進後退
        if((movePos.transform.position - transform.position).magnitude > maxRange)
        {
            //前後移動
            velosity += transform.forward * forwardSpeed;
        }
        else if((movePos.transform.position - transform.position).magnitude < minRange)
        {
            //前後移動
            velosity += -transform.forward * forwardSpeed;
        }
        
        //左右移動
        velosity += transform.right * rightSpeed * BoolToInt(rightFlag);
    }

    //左右移動の変更
    IEnumerator RightLeft()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            rightFlag = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        }
    }

    //goがセンターの方向に向いているか angleが誤差分
    bool AngleFlag(GameObject go,GameObject center,float angle)
    {
        Vector3 dif = center.transform.position - go.transform.position;
        // ラジアン
        float radian = -Mathf.Atan2(dif.z, dif.x);
        float degree = (radian * Mathf.Rad2Deg);
        degree += 90f;  //degreeはgoがどれだけ向いたら向けるか

        //角度を出す
        float angle2 = Mathf.DeltaAngle(go.transform.eulerAngles.y, degree);
        
        if(angle2 < angle && angle2 > -angle)
        {
            return true;
        }
        return false;
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

    //壁にぶつかると反対方向へ
    void OnCollisionEnter(Collision col)
    {
        rightFlag = !rightFlag;
    }

    //Bugのリジッドボディ追加
    void BugRigidbody(Rigidbody rBody)
    {
        rBody.useGravity = false;
        //速度の追加
        rBody.velocity = rBody.gameObject.transform.forward * BUG_SPEED;
    }

    //重力の処理
    void GravityControl()
    {
        //地上
        if (isGround)
        {
            gravity = 0f;
        }
        //空中
        else
        {
            gravity += GRAVITY_SIZE * Time.deltaTime;
        }
    }

    //設置判定
    void IsGround()
    {
        if (Physics.Linecast(transform.position + new Vector3(0f, 0.1f, 0f), transform.position - new Vector3(0f, 0.25f, 0f), LayerMask.GetMask("Ground", "Block", "CableBlock")) && !jumpFlag)
        {
            isGround =  true;
            return;
        }
        isGround = false;
        return;
    }

    //ここから下は全部攻撃関連---------------------------------------------------------------------------------------

    //攻撃の種類変更
    void AttackModeChange()
    {
        //1回は必ず繰り返すwhile
        do
        {
            attackMode = (AttackMode)Enum.ToObject(typeof(AttackMode), UnityEngine.Random.Range(0, attackNum));
        }
        while (AttackCancel());
    }

    //処理ができないものはtrueを返す
    bool AttackCancel()
    {
        if(attackMode == AttackMode.blow && (player.transform.position - transform.position).magnitude > BLOW_RANGE)
        {
            return true;
        }
        return false;
    }

    void Attack()
    {
        switch (attackMode)
        {
            //バグ投げ付けの処理
            case AttackMode.bug:
                AttackBug();
                break;
            //糸でのプレイヤー移動速度減少
            case AttackMode.net:
                AttackNet();
                break;
            case AttackMode.blow:
                AttackBlow();
                break;
        }
    }

    //攻撃の処理------------------------------------------------

    //バグの投げ付け
    void AttackBug()
    {
        GameObject minGo = null;
        float range = 0f;

        //バグブロックの中から
        foreach (Transform child in bugBlock.transform)
        {
            //一番近いバグブロックを探す
            if (range == 0 || range > (transform.position - child.transform.position).magnitude)
            {
                range = (transform.position - child.transform.position).magnitude;
                minGo = child.gameObject;
            }
        }
        //糸を吐いていないとき
        if (!bugFlag)
        {
            //Bugをとれる距離まで行く
            if ((transform.position - minGo.transform.position).magnitude > BUG_HAVE_RANGE || (transform.position - minGo.transform.position).magnitude < BUG_HAVE_MIN_RANGE)
            {
                PositonMove(minGo, BUG_HAVE_MIN_RANGE, BUG_HAVE_RANGE, ROTATE_BUG, FORWARD_BUG_SPEED, RIGHT_BUG_SPEED);
            }
            //角度を合っているかの確認
            else if (AngleFlag(gameObject, minGo, ANGLE_SAFE))
            {
                //糸の生成
                GameObject go = Instantiate(netPrefab);
                go.transform.parent = netPos.transform;
                go.transform.position = netPos.transform.position;
                go.transform.LookAt(minGo.transform.position);
                bugFlag = true;
            }
            //あっていなければ回転する
            else
            {
                PositonMove(minGo, BUG_HAVE_MIN_RANGE, BUG_HAVE_RANGE, ROTATE_BUG, 0f, 0f);
            }
        }
        //糸をブロックもっていていつでも投げれるとき
        else if (BugBlockFlag)
        {
            //角度を合っているかの確認
            if (AngleFlag(gameObject, player, ANGLE_SAFE))
            {
                //バグの発射
                foreach (Transform child in transform)
                {
                    if (child.gameObject.name == "NetPos")
                    {
                        foreach (Transform child2 in child.transform)
                        {
                            if (child2.gameObject.tag == "Bug")
                            {
                                //バグの発射
                                for (int i = 0; i < bulletNum; i++)
                                {
                                    GameObject go = Instantiate(bugBulletPrefab);
                                    //場所の変更
                                    go.transform.position = child2.transform.position;
                                    //角度を一定に
                                    go.transform.eulerAngles =
                                        child2.transform.eulerAngles + new Vector3(0f, -BUG_ANGLE / 2 + (BUG_ANGLE / bulletNum * i), 0f);
                                    //リジッドボディの追加
                                    Rigidbody goBody = go.AddComponent<Rigidbody>();
                                    BugRigidbody(goBody);
                                    //10秒後に削除
                                    Destroy(go, 10f);
                                }
                                //ブロックの削除
                                Destroy(child2.gameObject);
                            }
                        }
                    }
                    else if (child.gameObject.name == "NetParent")
                    {
                        foreach (Transform child2 in child.transform)
                        {
                            //糸の削除
                            Destroy(child2.gameObject);
                        }
                    }
                }
                BugBlockFlag = false;
                attackFlag = false;
                bugFlag = false;
            }
            //あっていなければ回転しながらゆっくり近づく
            else
            {
                PositonMove(player, BUG_HAVE_MIN_RANGE, BUG_HAVE_RANGE, ROTATE_BUG, FORWARD_SPEED, RIGHT_SPEED);
            }
        }
    }

    //糸でのプレイヤー移動速度減少
    void AttackNet()
    {
        //角度を合っているかの確認
        if (AngleFlag(gameObject, player, ANGLE_SAFE))
        {
            //糸の生成
            GameObject go = Instantiate(trapNetPrefab);
            go.transform.position = netPos.transform.position;
            go.transform.LookAt(player.transform.position);
            attackFlag = false;
        }
        //あっていなければ回転する
        else
        {
            PositonMove(player, NET_MIN_RANGE, NET_RANGE, ROTATE_BUG, 0f, 0f);
        }
    }

    //体当たり
    void AttackBlow()
    {
        if(!rotateBlow)
        {
            if (AngleFlag(gameObject, player, ANGLE_SAFE))
            {
                rotateBlow = true;
            }
            else
            {
                //プレイヤーのほうを向く
                targetAngleRotate(gameObject, player.transform.position, ROTATE_BLOW);
            }
        }
        else
        {
            if(TimerCount(BLOW_TIME,ref timer))
            {
                //攻撃終了
                rotateBlow = false;
                attackFlag = false;
            }
            else
            {
                velosity += transform.forward * BLOW_SPEED; 
            }
        }
    }
}
