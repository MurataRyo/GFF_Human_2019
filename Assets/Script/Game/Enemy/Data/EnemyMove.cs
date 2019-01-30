using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyMove : MonoBehaviour
{
    GameController gCon;

    //アニメーション用
    [HideInInspector]
    public int playerState = 0;
    float speed = 0f;
    bool isGround = false;
    public float rotateSpeed = 0.4f;       //360°回る時間

    //現在の行動
    enum ModeNow
    {
        searchMode,     //索敵
        chaseMode,      //追跡
        attackMode      //攻撃
    }
    ModeNow modeNow = ModeNow.searchMode;   //現在何の行動をしているか
    ModeNow modeLog = ModeNow.attackMode;
    [HideInInspector]
    public bool attackFlag = false;         //攻撃中かどうか(硬直時もtrue)
    //敵の攻撃がプレイヤーに当たるかどうか
    [HideInInspector]
    public bool hitFlag = false;
    //いろいろと取得用-----------------------
    Rigidbody rBody;             //リジッドボディ
    public GameObject playerCharacter;  //プレイヤーキャラクター
    [HideInInspector]
    public Enemy enemy;          //取得用クラス
    public int life;
    //-------------------------------------
    float gravitySize = -9.81f;
    float gravity = 0f;
    float angleInterval = 5f;       //敵が索敵中に向きを変える頻度
    float timer = 0f;               //時間
    const float NAV_TIMER_MAX = 0.5f;
    bool navPosFlag = true;         //ナビゲーションに従うかどうか
    Vector3 defaultPosition;        //敵の初期位置
    Vector3 nextMovePosition;       //向かっている場所
    Vector3 objectVelosity;         //オブジェクトの力を反映
    public Vector3 windVelosityLog;
    Vector3 navNextPosition;
    public bool windFlag;
    Type type;
    // Use this for initialization
    public virtual void Start()
    {
        gCon = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        defaultPosition = transform.position;

        rBody = GetComponent<Rigidbody>();

        playerCharacter
            = GameObject.FindGameObjectWithTag("Player");　//プレイヤーの取得

        foreach (Transform child in playerCharacter.transform)
        {
            if (child.gameObject.name == "PlayerCharacter")
            {
                playerCharacter = child.gameObject;
            }
        }

        int count = 0;   //繰り返す用の変数

        //敵のデータが取れるまで繰り返す
        while (count < EnemyDataLoad.EnemyData2.data.Length)
        {
            //テキストの名前（Textの0番目）とオブジェクト名が一緒のとき
            if (transform.name == EnemyDataLoad.EnemyData2.data[count].enemyName)
            {
                //読み込み
                enemy = EnemyDataLoad.EnemyData2.data[count];
                break;
            }
            count++;
        }
        //ライフの追加
        life = new int();
        life = enemy.life;
        //初期スピードの設定
        speed = enemy.seachSpeed;
        RandomPositonNext();
    }


    void Update()
    {
        //重力の処理
        GravityControl();

        //Rayの接地判定が地面なら
        if (isGround && !attackFlag)
        {
            NavMeshHit navHit;
            //敵とプレイヤーの下にNavMeshがある場合
            if (!NavMesh.Raycast(transform.position + new Vector3(0f, 0.1f, 0f),
                transform.position - new Vector3(0f, 0.25f, 0f), out navHit, NavMesh.AllAreas) &&
                !NavMesh.Raycast(playerCharacter.transform.position + new Vector3(0f, 0.1f, 0f),
                playerCharacter.transform.position - new Vector3(0f, 0.25f, 0f), out navHit, NavMesh.AllAreas))
            {
                //どの動きをするかの更新
                ModeUpDate();
            }
            else if (modeNow == ModeNow.attackMode)
            {
                modeNow = ModeNow.searchMode;
            }

            switch (modeNow)
            {
                //索敵
                case ModeNow.searchMode:
                    SearchMove();
                    break;
                //追跡
                case ModeNow.chaseMode:
                    ChasehMove();
                    break;
                //攻撃
                case ModeNow.attackMode:
                    attackMove();
                    break;
            }
        }


        Animator animator = GetComponent<Animator>();
        animator.SetInteger("PState", playerState);

        //移動
        NavUpdate();

        Vector3 velocity = Vector3.zero;
        if (modeNow != ModeNow.attackMode && !windFlag)
        {
            //移動方向を向く
            targetAngleRotate(gameObject, navPosFlag ? navNextPosition : nextMovePosition, rotateSpeed);
            //正面に歩く
            velocity = transform.forward * speed;

        }

        rBody.velocity = new Vector3(velocity.x, gravity, velocity.z) + objectVelosity +
            (windFlag ? new Vector3(windVelosityLog.x, 0f, windVelosityLog.z) : Vector3.zero);

        //敵の消滅処理
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        navPosFlag = true;
        if (windFlag && isGround)
        {
            windVelosityLog = Vector3.zero;
            windFlag = false;
        }
        objectVelosity = Vector3.zero;
        RayGround(transform.position);
    }

    //モードの更新
    void ModeUpDate()
    {
        //攻撃範囲内
        if (NavPositionFlag(transform.position, playerCharacter.transform.position, enemy.attackRange))
        {
            //攻撃モード
            modeNow = ModeNow.attackMode;
        }
        //追跡範囲内
        else if (NavPositionFlag(transform.position, playerCharacter.transform.position, enemy.searchRange))
        {
            //追跡モード
            modeNow = ModeNow.chaseMode;
        }
        //それ以外
        else
        {
            //索敵モード
            modeNow = ModeNow.searchMode;
        }

        //敵のモードが変更されたとき
        if (modeLog != modeNow)
        {
            ModeChange();
        }

        modeLog = modeNow;  //ログの取得
    }

    //敵のモードが変更されたときの処理
    void ModeChange()
    {
        if (modeNow == ModeNow.searchMode)
        {
            //スピードの変更
            speed = enemy.seachSpeed;
            //向かう場所の指定
            RandomPositonNext();
        }
        else if (modeNow == ModeNow.chaseMode)
        {
            //スピードの変更
            speed = enemy.chaseSpeed;
        }
    }

    //距離の判定
    bool NavPositionFlag(Vector3 start, Vector3 end, float range)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(playerCharacter.transform.position, out hit, Mathf.Infinity, NavMesh.AllAreas);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(this.transform.position, hit.position, NavMesh.AllAreas, path);

        //最終地点がプレイヤーでなければ終了
        if (path.corners.Length == 0 ||
            !Tolerance(path.corners[path.corners.Length - 1].x, playerCharacter.transform.position.x, 0.1f) ||
            !Tolerance(path.corners[path.corners.Length - 1].z, playerCharacter.transform.position.z, 0.1f))
        {
            return false;
        }

        float length = 0f;
        length += (transform.position - path.corners[0]).magnitude;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += (path.corners[i - 1] - path.corners[i]).magnitude;
        }

        if (length < range)
        {
            return true;
        }
        return false;
    }

    bool Tolerance(float left, float right, float tolerance)
    {
        if (left + tolerance > right &&
            left - tolerance < right)
        {
            return true;
        }
        return false;
    }

    //距離の判定
    bool PositionFlag(Vector3 start, Vector3 end, float range)
    {
        Vector2 s = new Vector2(start.x, start.z);
        Vector2 e = new Vector2(end.x, end.z);
        float position = (s - e).magnitude;
        if (position < range)
        {
            return true;
        }
        return false;
    }

    //索敵中------------------------------------------------------------------------------
    void SearchMove()
    {
        //条件を満たすと索敵場所変更
        if (TimerCount(angleInterval, ref timer) //一定時間経過
            || PositionFlag(transform.position, nextMovePosition, 4f)) //指定した場所に着く
        {
            RandomPositonNext();
        }
    }

    //自分の持ち場あたりをランダムで徘徊
    void RandomPositonNext()
    {
        timer = 0f;
        nextMovePosition = new Vector3(defaultPosition.x + UnityEngine.Random.Range(-5f, 5f),
            defaultPosition.y,
            defaultPosition.z + UnityEngine.Random.Range(-5f, 5f));
    }

    //追跡中--------------------------------------------------------------------------------
    void ChasehMove()
    {
        nextMovePosition = playerCharacter.transform.position;
    }

    //攻撃範囲------------------------------------------------------------------------------
    void attackMove()
    {
        Attack();
    }
    //ナビゲーションの更新
    void NavUpdate()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(nextMovePosition, out hit, Mathf.Infinity, NavMesh.AllAreas);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(this.transform.position, hit.position, NavMesh.AllAreas, path);

        navNextPosition = this.transform.position;

        for (int i = 0; i < path.corners.Length; i++)
        {
            if ((path.corners[i] - path.corners[0]).magnitude > 0.5f)
            {
                navNextPosition = path.corners[i];
                break;
            }
            navNextPosition = path.corners[i];
        }
    }

    //重力の処理
    void GravityControl()
    {
        //地上
        if (isGround)
        {
            gravity = enemy.fryFlag ? 0 : gravitySize * Time.deltaTime;
        }
        //空中
        else
        {
            gravity += gravitySize * Time.deltaTime;
        }
    }

    //--------------------------------------------------------------------------------------

    //時間関係
    bool TimerCount(float time, ref float timer)
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            return true;
        }
        return false;
    }

    //レイでの接地判定
    void RayGround(Vector3 start)
    {
        isGround = false;
        NavMeshHit navHit;
        RaycastHit hit;
        if (Physics.Linecast(start + new Vector3(0f, 0.1f, 0f), start - new Vector3(0f, 0.25f, 0f), out hit, LayerMask.GetMask("Ground", "Block", "CableBlock", "Transparent")))
        {
            isGround = true;
        }
        //NavMeshが下にあるかどうかの確認
        //ある場合
        else if (!NavMesh.Raycast(start + new Vector3(0f, 0.1f, 0f), start - new Vector3(0f, 0.25f, 0f), out navHit, NavMesh.AllAreas))
        {
            if (enemy.fryFlag &&
                navHit.distance <= 0.25f)
            {
                isGround = true;
                return;
            }
            else
            {
                navPosFlag = false;
                nextMovePosition = defaultPosition;
            }
        }
        return;
    }

    //当たっているとき
    public void WindTrigger(GameObject go)
    {
        //その力を返す
        objectVelosity += go.GetComponent<VelosityBlock>().VelosityObject(ref gravity);
    }

    //Centerの場所へMoveObをSpeed(360°回る時間)の速さで向く。
    void targetAngleRotate(GameObject MoveOb, Vector3 Center, float Speed)
    {
        float angleY;
        bool ine;

        Vector3 dif = Center - MoveOb.transform.position;

        //ラジアン
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

    public virtual void Attack()
    {

    }

    void OnCollisionStay(Collision col)
    {
        //敵に当たっていたら場所変更
        if (col.gameObject.tag == "Enemy")
        {
            RandomPositonNext();
        }

        if (!enemy.fryFlag && col.gameObject.GetComponent<VelosityBlock>() && !col.collider.isTrigger)
        {
            //プレイヤー以外なら終了（ブロック）
            for (int i = 0; i < col.contacts.Length; i++)
            {
                //プレイヤーがコンベヤーの上にいるとき
                if (col.contacts[i].point.y <= transform.position.y + 0.1f)
                {
                    //その力を返す
                    objectVelosity = col.gameObject.GetComponent<VelosityBlock>().VelosityObject(ref gravity);
                    break;
                }
            }
        }
    }

    public void DestoryEnemy()
    {
        gCon.SeSoundLoad(AudioController.Se.Enemy_Destory);
        GameObject go = Instantiate(Resources.Load<GameObject>("GameObject/Particle/Extinction"));
        go.transform.position = transform.position;
        Destroy(gameObject);
    }
}

[Serializable]
public class Enemy
{
    public string enemyName;         //名前
    public int life;                 //ライフ
    public float searchRange;        //索敵範囲
    public float attackRange;        //攻撃に移る範囲
    public float seachSpeed;         //索敵中の速度
    public float chaseSpeed;         //プレイヤーを追っているときの速度
    public float attackModeSpeed;    //攻撃モーションスタートから攻撃判定が生まれるまでの時間
    public float attackSpeed;        //攻撃モーションの時間
    public float attackInterval;     //攻撃してから次の行動に移るまでの時間
    public float startTime;          //攻撃範囲に入ってから攻撃スタートするまでの時間
    public bool fryFlag;             //空を飛ぶ敵かどうか
    public bool rangeFlag;           //近距離攻撃か遠距離攻撃か trueが遠距離 falseが近距離
}


