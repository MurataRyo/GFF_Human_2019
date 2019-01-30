using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // 参照➝gomafrontier.com/unity/1585
    float speed;                                //カメラの現在速度
    [SerializeField] float ROTATE_SPEED_TPS;    // TPSカメラが回る速度
    [SerializeField] float ROTATE_SPEED_FPS;    // FPSカメラが回る速度
    [SerializeField] float assistSpeed;
    public LayerMask layerMask;
    [SerializeField] GameObject player;               // 自機
    [SerializeField] GameObject mainCamera;           // カメラ(三人称)
    PlayerMove playerScript;

    float cameraMaxDistance;     //肩越しカメラと頭の上との距離
    [HideInInspector] public bool lookOnFlag = false;         //true = FPS false = TPS

    const float FPS_MAX_DISTANCE = -0.35f;   //FPS時のカメラとキャラクターの距離
    const float TPS_MAX_DISTANCE = 6f;      //FPS時のカメラとキャラクターの距離

    GameController gameController;          //取得用
    XBox xBox;

    private const float ANGLE_LIMIT_UP = 60f;       // カメラの上回転の限度
    private const float ANGLE_LIMIT_DOWN = -60f;    // カメラの下回転の限度

    [HideInInspector] public bool heightLock = false;
    [HideInInspector] public float height = 0f;

    void Start()
    {
        cameraMaxDistance = TPS_MAX_DISTANCE;
        playerScript = player.GetComponent<PlayerMove>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        xBox = gameController.GetComponent<XBox>();
    }

    void Update()
    {
        if (!heightLock && playerScript.playerLife != 0)
        {
            // カメラの切り替え
            if(!playerScript.stopPlayer)
            {
                if (playerScript.cableScript.chilledBlock == null && !playerScript.cableScript.cableLock && (xBox.Button(XBox.Str.LB)))
                {
                    lookOnFlag = true;
                }
                else
                {
                    lookOnFlag = false;
                }
            }
            

            //FPSとTPSでカメラの高さを変更する
            transform.position = player.transform.position + (lookOnFlag ? new Vector3(0f, 1.15f, 0f) - transform.forward * 0.15f : new Vector3(0f, 1.5f, 0f));     // カメラを自機に追従させる
            if (gameController.gameMode == GameController.GameMode.play)
            {
                float cameraDistance;

                //FPS時の処理
                if (lookOnFlag)
                {
                    RaycastHit hit;
                    cameraMaxDistance = Mathf.MoveTowards(cameraMaxDistance, FPS_MAX_DISTANCE, 60f * Time.deltaTime);

                    if (cameraMaxDistance != FPS_MAX_DISTANCE &&
                        Physics.SphereCast(this.transform.position, 0.02f, -transform.forward, out hit, cameraMaxDistance, layerMask))
                    {
                        cameraMaxDistance = hit.distance;
                    }
                    else
                    {
                        mainCamera.transform.position = player.transform.position + (Vector3.up * 1.15f) + -player.transform.forward * (cameraMaxDistance - 0.15f);
                    }
                    mainCamera.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);
                }
                //TPS時の処理
                else
                {
                    //TPS時
                    RaycastHit hit;
                    cameraMaxDistance = Mathf.MoveTowards(cameraMaxDistance, TPS_MAX_DISTANCE, 60f * Time.deltaTime);
                    if (Physics.SphereCast(this.transform.position, 0.02f, -transform.forward, out hit, cameraMaxDistance, layerMask))
                    {
                        cameraMaxDistance = hit.distance;
                    }
                    cameraDistance = cameraMaxDistance;
                    //tpsに移行が終わってる時
                    if (cameraMaxDistance == TPS_MAX_DISTANCE)
                    {
                        //1回目は-0.05したくないので余分に足しておく
                        Vector3 foward = this.transform.forward += new Vector3(0f, 0.05f, 0f);
                        //めり込まないところを探す
                        do
                        {
                            foward -= new Vector3(0f, 0.05f, 0f);
                            foward.Normalize();
                            if (Physics.SphereCast(this.transform.position, 0.02f, -foward, out hit, cameraMaxDistance, layerMask))
                            {
                                cameraDistance = hit.distance;
                            }
                        }
                        //無限ループしないように上限を設定する
                        while (cameraDistance <= 1.5f && foward.y >= -0.95);
                        //カメラの向きと場所の変更
                        mainCamera.transform.position = transform.position + -foward * cameraDistance;
                    }
                    else
                    {
                        //カメラの向きと場所の変更
                        mainCamera.transform.position = transform.position + -transform.forward * cameraDistance;
                    }
                    mainCamera.transform.LookAt(transform.position);
                    transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
                }
                rotateCameraAngle();

                if (playerScript.cableScript.chilledBlock == null && !playerScript.cableScript.cableLock &&
                    xBox.ButtonDown(XBox.Str.LB) && playerScript.cableFlag && !playerScript.stopPlayer)
                {
                    mainCamera.transform.LookAt(playerScript.cableScript.cablePosition.transform.position);
                }
            }
        }
    }

    //誤差の計算
    bool AboutFloat(float mySelf, float target, float value)
    {
        if (mySelf <= target + value &&
           mySelf >= target - value)
        {
            return true;
        }
        return false;
    }

    //360度から-180度方式への変換
    float Angle360To180(float degree)
    {
        if (degree < 180)
        {
            return degree;
        }

        return 360 - degree;
    }

    private void rotateCameraAngle()
    {
        //3人称カメラやケーブルを出していないときなら変更可能
        if (!playerScript.stopPlayer && (!lookOnFlag || !playerScript.cableFlag))
        {
            //エイムアシスト対象外ならスピードをカメラ基準に
            if (!AimAssist())
            {
                speed = lookOnFlag ? ROTATE_SPEED_FPS : ROTATE_SPEED_TPS;
            }

            // カメラの回転
            Vector3 angle = new Vector3
                ((Input.GetAxis((XBox.AxisStr.RightJoyRight).ToString())) * speed * Time.deltaTime,   // 左右
                Input.GetAxis((XBox.AxisStr.RightJoyUp).ToString()) * speed * Time.deltaTime,    // 上下
                0f);
            transform.eulerAngles += new Vector3(angle.y, angle.x);
        }

        // Transform.eulerAngles.xが180度以上であれば360度減算する
        float angle_x =
            180f <= transform.eulerAngles.x ?   // Transform.eulerAngles.xが180度以上であるかどうかを判定する
            transform.eulerAngles.x - 360 :     // 180度以上のとき
            transform.eulerAngles.x;            // そうでないとき
        transform.eulerAngles = new Vector3(
            Mathf.Clamp(angle_x, ANGLE_LIMIT_DOWN, ANGLE_LIMIT_UP),
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );
    }

    //AIMアシスト
    bool AimAssist()
    {
        RaycastHit hit;
        if (lookOnFlag && AssistRay(out hit))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                speed = assistSpeed;
                return true;
            }
            else if (hit.collider.gameObject.tag == "CableBlock" && hit.distance <= 15f)
            {

                speed = assistSpeed;
                return true;
            }
        }
        return false;
    }

    bool AssistRay(out RaycastHit hit)
    {
        return Physics.Raycast(
                        mainCamera.transform.position, //場所（カメラの位置からカメラとプレイヤーの距離分先）
                        mainCamera.transform.forward,  //カメラの向いている方向
                        out hit,
                        25f,                       //ケーブルの最長より高ければよし（？）
                        layerMask);   //カメラのレイヤーマスクと同じ
    }
}
