using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerMove : MonoBehaviour
{
    //被弾関係
    public enum PlayerState
    {
        none,       //通常時
        hit,         //無敵時
    }
    [HideInInspector]
   　public PlayerState playerState = PlayerState.none;

    [HideInInspector] public bool cableFlag = false;                      // ケーブルを投げているか（物を持っていないとき）
    [HideInInspector] public Vector3 windVelosityLog = Vector3.zero;      // 風の力
    [HideInInspector] public bool windFlag = false;                       // 風に当たっていたかどうか

    [SerializeField] float TPS_SPEED;                   // 自機の速度(三人称)
    [SerializeField] float FPS_SPEED;                   // 自機の速度(一人称)
    [SerializeField] float TRAP_SPEED;                  // 自機の速度(糸の上)
    [SerializeField] GameObject cameraObject;           // カメラの親オブジェクト
    [SerializeField] GameObject cable;                  // ケーブル
    [SerializeField] GameObject cableUsb;               // ケーブルのUSB
    [SerializeField] Image Life;                        // 自機の体力のUI
    [SerializeField] Image LostLife;                    // 自機の体力のUI（空）
    [SerializeField] Image Bullet;                      // 自機の弾のUI
    [SerializeField] Image EmptyBullet;                 // 自機の弾のUI（空）
    [SerializeField] Transform muzzle;                  // 発射する座標
    [SerializeField] SkinnedMeshRenderer gumModel;      // 銃のモデル
    [SerializeField] GameObject playerBullet;           // 自機の弾
    [SerializeField] Image pointer;                     // 照準
    [SerializeField] LayerMask blockMask;

    GameController gameController;  //ゲームコントローラー
    RectTransform lifeRect;
    RectTransform bulletRect;
    RectTransform lostLifeRect;
    RectTransform emptyBulletRect;

    [HideInInspector] public Cable cableScript;
    CameraScript cameraScript;
    XBox xBox;

    float speed;                                // 自機の速度
    new Rigidbody rigidbody;
    const float HIT_TIME = 1.0f;                // 無敵時間
    const float MAGMA_HIT_TIME = 0.6f;
    const float CHARACTER_ROTATE_SPEED = 0.2f;  //キャラクターの回転速度(1回転するのにかかる時間)
    const int PLAYER_LIFE_MAX = 5;
    const int BULLETS_MAX = 10;


    //重力関連---------------------------------------------------------------------

    public Vector3 objectVelosity = Vector3.zero;   // オブジェクトから受ける力
    Vector3 windVelosity = Vector3.zero;     // 風の力
    public float gravity = 0f;                      // 現在の重力
    float gravitySize = -9.81f;              // 重力の大きさ
    const float DEATH_HEIGHT = -6f;          // 死ぬ高さ（地面からの距離。落下しではない）
    [HideInInspector]
    public bool isGround = false;           // 接地しているかどうか
    [HideInInspector]
    public bool trapFlag = false;           // 蜘蛛の糸に接地しているかどうか

    float timer = 0f;    //時間関係
    float airTime;
    [HideInInspector] public int playerLife;                   // 自機の残り体力
    int bullets;                      // 自機の残り弾数
    private Animator animator;
    bool waterFlag = false;
    bool clearFlag = false;

    [HideInInspector] public bool stopPlayer = false;

    void Start()
    {
        playerLife = PLAYER_LIFE_MAX;
        bullets = BULLETS_MAX;
        cableScript = cable.GetComponent<Cable>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        pointer.enabled = false;
        cameraScript = cameraObject.transform.parent.GetComponent<CameraScript>();
        speed = TPS_SPEED;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        lifeRect = Life.GetComponent<RectTransform>();
        lifeRect.sizeDelta = new Vector2(1465 * playerLife, 1249);
        bulletRect = Bullet.GetComponent<RectTransform>();
        bulletRect.sizeDelta = new Vector2(250 * bullets, 1095);
        lostLifeRect = LostLife.GetComponent<RectTransform>();
        lostLifeRect.sizeDelta = new Vector2(1465 * (PLAYER_LIFE_MAX - playerLife), 1249);
        emptyBulletRect = EmptyBullet.GetComponent<RectTransform>();
        emptyBulletRect.sizeDelta = new Vector2(250 * (BULLETS_MAX - bullets), 1095);

        xBox = gameController.GetComponent<XBox>();
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        PointerMove();
        //プレイヤーが自分で動く力
        Vector3 velocity = Vector3.zero;
        if (waterFlag)
        {
            //重力の処理
            GravityControl();
        }

        //動けないタイミング
        if (!stopPlayer)
        {
            //重力の処理
            GravityControl();
            //ケーブルを投げる処理
            CableMove();

            // 自機の移動速度を変える
            if (trapFlag && isGround) // 接地していて、かつ糸の上のとき
            {
                speed = TRAP_SPEED;
            }
            else if (cameraScript.lookOnFlag) // ↑の条件を満たさず、かつカメラが一人称視点のとき
            {
                speed = FPS_SPEED;
            }
            else // それ以外のとき
            {
                speed = TPS_SPEED;
            }

            //ケーブルを投げているときにはできない動き
            if (!cableFlag)
            {
                Vector2 vec2 = new Vector2(-Input.GetAxis((XBox.AxisStr.LeftJoyUp).ToString()), Input.GetAxis((XBox.AxisStr.LeftJoyRight).ToString())).normalized;
                // カメラの向きを基準に自機を移動 Verticalキーで前後 Horizontalキーで左右
                velocity =
                    (new Vector3(cameraObject.transform.forward.x, 0f, cameraObject.transform.forward.z).normalized * vec2.x +  // 前後
                    cameraObject.transform.right * vec2.y);
                // 移動速度
                velocity *= speed;

                // カメラが肩ごし視点のときかケーブルを出しているとき
                if (cameraScript.lookOnFlag || cable.activeSelf)
                {
                    // 自機の向きをカメラに合わせる
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, cameraObject.transform.eulerAngles.y, transform.eulerAngles.z);
                }
                else if (!cameraScript.lookOnFlag)  // カメラが三人称視点のとき
                {
                    // プレイヤーが少しでも動いているとき
                    if (velocity != Vector3.zero)
                    {
                        Vector3 rotation =
                            (cameraObject.transform.forward * -Input.GetAxis((XBox.AxisStr.LeftJoyUp).ToString())) + // 前後 
                            (cameraObject.transform.right * Input.GetAxis((XBox.AxisStr.LeftJoyRight).ToString()));  // 左右
                        rotation.y = 0f;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rotation), (360f / CHARACTER_ROTATE_SPEED) * Time.deltaTime);
                    }
                }
            }

            // アニメーション関連-------------------------------------------------------------
            //（歩き）
            animator.SetBool("Move", !cableFlag && velocity != Vector3.zero);
            //（射撃）
            animator.SetBool("LookOn", cableFlag || cameraScript.lookOnFlag);
            animator.SetBool("Shot", cableFlag);
            //（接地）
            animator.SetBool("IsGround", isGround);
            animator.SetFloat("AirTime", airTime);
            airTime += Time.deltaTime;

            gumModel.enabled = cameraScript.lookOnFlag || cable.activeSelf;

        }

        //windFlagがtrueならwindVelosityLogをfalseならwindVelosityをくわえる
        rigidbody.velocity = new Vector3(velocity.x, gravity + velocity.y, velocity.z) +
                objectVelosity + (windFlag ? new Vector3(windVelosityLog.x, 0f, windVelosityLog.z) : windVelosity); //自分機にかかる力



        // 自機のライフが０になっているとき
        if (playerLife <= 0 || transform.position.y < DEATH_HEIGHT)
        {
            Dead();
        }
        //無敵判定関連
        else if (playerState != PlayerState.none)
        {
            timer += Time.deltaTime;
            if (timer > HIT_TIME)
            {
                playerState = PlayerState.none;
                timer = 0f;
            }
        }

    }

    public void Reload()
    {
        if (bullets < 10)
        {
            bullets = BULLETS_MAX;
            bulletRect.sizeDelta = new Vector2(250 * bullets, 1095);
            emptyBulletRect.sizeDelta = new Vector2(250 * (BULLETS_MAX - bullets), 1095);
        }
    }

    void Dead()
    {
        stopPlayer = true;
        playerLife = 0;
        lifeRect.sizeDelta = new Vector2(1465 * playerLife, 1249);
        lostLifeRect.sizeDelta = new Vector2(1465 * (PLAYER_LIFE_MAX - playerLife), 1249);
        if (!gameController.coroutineFlag)
        {
            gameController.BlackOutAlpha(true, 0f, false, Color.black, GameController.BlackOutTime);
        }
    }

    //Updateとは別で一定時間で呼び出される
    //TriggerEnterなどの直前
    void FixedUpdate()
    {
        isGround = false;
        trapFlag = false;
        windVelosity = Vector3.zero;
        objectVelosity = Vector3.zero;
    }

    //重力の処理
    void GravityControl()
    {
        //地上
        if (isGround)
        {
            gravity = gravitySize * Time.deltaTime;
        }
        //空中
        else
        {
            gravity += gravitySize * Time.deltaTime;
        }
    }

    //ケーブルの処理
    void CableMove()
    {
        if (isGround)
        {
            if ((xBox.ButtonDown(XBox.Str.RB)))
            {
                //ケーブルの発射
                if (!cable.activeSelf)
                {
                    if (cameraScript.lookOnFlag)
                    {
                        cableFlag = true;
                        cable.SetActive(true);
                        cableUsb.SetActive(true);
                        RaycastHit hit;
                        if (CameraRay(out hit, 15f, cameraScript.layerMask))
                        {
                            //ケーブルが何かに当たるなら当たった場所に向く
                            cable.transform.LookAt(hit.point);
                        }
                        else
                        {
                            //そうでなければ大体向いている方向に打つ
                            cable.transform.eulerAngles = cameraObject.transform.eulerAngles;
                        }
                        cable.GetComponent<Cable>().DefaultLoad();
                    }
                }
                else
                {
                    cable.GetComponent<Cable>().DestoryFlag();
                }
            }
            else if (xBox.ButtonDown(XBox.AxisStr.BackTrigger, false))
            {
                // 弾の発射
                if (!cable.activeSelf && cameraScript.lookOnFlag && bullets > 0)
                {
                    gameController.SeSoundLoad(AudioController.Se.Player_Gun);
                    bullets--;
                    bulletRect.sizeDelta = new Vector2(250 * bullets, 1095);
                    emptyBulletRect.sizeDelta = new Vector2(250 * (BULLETS_MAX - bullets), 1095);
                    GameObject go = Instantiate(playerBullet);
                    go.tag = "PlayerBullet";
                    go.transform.position = muzzle.position;
                    RaycastHit hit;

                    if (CameraRay(out hit, 25f, cameraScript.layerMask))
                    {
                        //ケーブルが何かに当たるなら当たった場所に向く
                        muzzle.transform.LookAt(hit.point);
                    }
                    else
                    {
                        //そうでなければ大体向いている方向に打つ
                        muzzle.transform.eulerAngles = cameraObject.transform.eulerAngles;
                    }
                    go.GetComponent<Rigidbody>().velocity += muzzle.forward * 20f;  // 銃口の方向に飛ばす
                }
            }
        }
    }

    void PointerMove()
    {
        //照準の適応
        if (stopPlayer)
        {
            if (pointer.enabled)
            {
                pointer.enabled = false;
            }
        }
        else if (cameraScript.lookOnFlag != pointer.enabled)
        {
            pointer.enabled = cameraScript.lookOnFlag;
        }
    }

    bool CameraRay(out RaycastHit hit, float distance, LayerMask mask)
    {
        return Physics.Raycast(
                        cameraObject.transform.position, //場所（カメラの位置からカメラとプレイヤーの距離分先）
                        cameraObject.transform.forward,  //カメラの向いている方向
                        out hit,
                        distance,                       //ケーブルの最長より高ければよし（？）
                        mask);   //カメラのレイヤーマスクと同じ
    }

    //レイでの接地判定
    public bool RayGround(Vector3 start, Vector3 end, out RaycastHit hit)
    {
        if (Physics.Linecast(start, end, out hit, LayerMask.GetMask("Ground", "Block", "CableBlock", "Net", "Transparent")))
        {
            return true;
        }
        return false;
    }

    //レイでの接地判定
    bool RayGround(Vector3 start, float range, out RaycastHit hit, LayerMask mask)
    {
        if (Physics.Raycast(start + new Vector3(0f, range, 0f), -transform.up, out hit, range + 1f, mask))
        {
            return true;
        }
        return false;
    }

    //Scene移動時
    void SceneLoad(string str)
    {
        LoadScene.sceneName = str;
        SceneManager.LoadScene("Load");
    }

    //風ギミックの処理
    public void WindTrigger(GameObject go)
    {
        //その力を返す
        windVelosity += go.GetComponent<VelosityBlock>().VelosityObject(ref gravity);
    }

    //接地判定
    void IsGround(Collision col)
    {
        if (!isGround)
        {
            for (int i = 0; i < col.contacts.Length; i++)
            {
                if (col.contacts[i].normal.y > 0.5f)
                {
                    airTime = 0f;
                    isGround = true;
                    return;
                }
            }
        }
    }

    // 当たった瞬間
    void OnCollisionEnter(Collision col)
    {
        //接地判定
        IsGround(col);
    }

    //当たっている時
    void OnCollisionStay(Collision col)
    {
        //プレイヤー以外なら終了（ブロック）
        for (int i = 0; i < col.contacts.Length; i++)
        {
            if (col.contacts[i].thisCollider.gameObject.tag != "Player")
            {
                if (col.contacts[i].thisCollider.gameObject.tag == "CableMoveBlock")
                {
                    cableScript.chilledBlock.GetComponent<CableMoveBlock>().IsGround(col);
                }
                return;
            }
        }
        //接地判定
        IsGround(col);

        //当たったものの確認
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Block" || col.gameObject.tag == "CableBlock" ||
            col.gameObject.tag == "CableSwitchBlock" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "Net" ||
            col.gameObject.tag == "Gimmick" || col.gameObject.tag == "CableMoveBlock")
        {
            if (!col.collider.isTrigger)
            {
                windFlag = false;
            }
            if (col.gameObject.GetComponent<VelosityBlock>() && !col.collider.isTrigger)
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
    }

    //溶岩に触れた処理
    public void TriggerVolcano()
    {
        stopPlayer = true;
        gravity = gravitySize * Time.deltaTime;
        cameraScript.heightLock = true;
        cameraScript.height = cameraObject.transform.position.y;
    }

    public void ExitVolcano()
    {
        stopPlayer = false;
        cameraScript.heightLock = false;
    }

    //水に触れた処理
    public void TriggerWater()
    {
        if (GetComponents<Collider>()[0].enabled)
        {
            for(int i = 0;i < GetComponents<Collider>().Length;i++)
            {
                GetComponents<Collider>()[i].enabled = false;
            }
        }
        waterFlag = true;
        stopPlayer = true;
        cameraScript.heightLock = true;
        cameraScript.height = cameraObject.transform.position.y;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "EndPosition" && !clearFlag)
        {
            StageClear();
        }
    }

    public void StageClear()
    {
        clearFlag = true;
        //クリア処理
        if ((int)GameController.stageNumber >= (int)GameController.clearNumber &&
            (int)GameController.clearNumber < Enum.GetValues(typeof(GameController.StageNumber)).Length)
        {
            GameController.clearNumber++;
        }
        //次のステージ
        if (!((int)GameController.stageNumber + 1 >= Enum.GetValues(typeof(GameController.StageNumber)).Length))
        {
            GameController.stageNumber++;
            gameController.SceneLoad("Stage");
        }
        //タイトル
        else
        {
            gameController.SceneLoad("Title");
        }
    }

    void OnTriggerStay(Collider col)
    {
        //溶岩ブロックなら
        if (col.gameObject.tag == "Volcano")
        {
            gravity = gravitySize * Time.fixedDeltaTime;
            if (timer < HIT_TIME - MAGMA_HIT_TIME)
            {
                timer = HIT_TIME - MAGMA_HIT_TIME;
            }
        }

        //if (col.gameObject.transform.parent && col.gameObject.transform.parent.transform.parent && col.gameObject.transform.parent.gameObject.transform.parent.GetComponent<QuickSand>())
        //{
        //    stopPlayer = true;
        //    col.gameObject.transform.parent.gameObject.transform.parent.GetComponent<QuickSand>().OnTrigger(gameObject, ref objectVelosity, ref gravity);
        //}

        //当たったのが敵でなおかつ攻撃判定なら
        if (col.gameObject.GetComponent<EnemyMove>() && col.gameObject.GetComponent<EnemyMove>().hitFlag)
        {
            Damage();
        }
    }

    //ダメージを受けるとき(無敵時間でもはいる)
    public void Damage()
    {
        //プレイヤーが通常時なら
        if (playerState == PlayerState.none)
        {
            gameController.SeSoundLoad(AudioController.Se.Player_Damage);
            if (playerLife > 0)
            {
                //赤から透明に
                gameController.BlackOutAlpha(false, 0.6f, false, Color.red, 1f);
                playerLife--;       // ダメージを受ける
                lifeRect.sizeDelta = new Vector2(1465 * playerLife, 1249);
                lostLifeRect.sizeDelta = new Vector2(1465 * (PLAYER_LIFE_MAX - playerLife), 1249);
            }
            playerState = PlayerState.hit;
        }
    }
}