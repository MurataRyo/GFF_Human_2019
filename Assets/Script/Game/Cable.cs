using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Cable : MonoBehaviour
{
    float usbRange;                     //現在のUSBのケーブルの長さ
    const float USB_ROTATE_SPEED = 5f;  //USB先端の回転速度（360°回るスピード）
    const float CABLE_ROTATE_SPEED = 5f;//USB本体の回転速度（360°回るスピード）
    const float MIN_SCALE = 1.0f;       //長さの最小値
    public const float MAX_SCALE = 10f; //長さの最大値
    const float SPEED = 15.0f;          //1秒間に延びる長さ(倍率)
    public GameObject cablePosition;           //ケーブルの先端
    public GameObject cablePositionDefault;    //ケーブルの初期位置
    [SerializeField]
    LayerMask blockMask;                //ブロックのマスク
    [SerializeField]
    LayerMask cableMask;                //CableBlockを入れる
    Vector3 nextPositon;                //次に行く予定の場所
    [SerializeField]
    GameObject playerCharacter;
    GameController gameController;          //取得用
    XBox xBox;

    List<Material> logMaterial;
    List<LayerMask> logLayer;
    List<GameObject> windUi;

    [SerializeField]
    GameObject buttonUi;

    GameObject electric;

    PlayerMove playerMove;

    float blockTimer = 0f;              //ブロックを手放すまでのカウント
    const float BLOCK_TIMER_MAX = 0.1f; //手放すまでの時間
    float timer = 0f;                   //時間用
    bool flag = true;                   //ケーブルの伸縮する方向(trueが伸びる。falseが縮む)
    bool cableFlag = false;             //trueなら伸縮
    public bool cableLock;              //スイッチに当たっているかどうか
    bool pawerFlag = false;             //電源を入れようとしているかどうか
    [HideInInspector] public GameObject chilledBlock;     //持っているブロックの情報
    [HideInInspector] public GameObject childPort;        //指しているポート
    [HideInInspector] public GameObject gimmickParent;
    PortBlock portBlockScript;
    List<GameObject> childGimmick = new List<GameObject>();

    const float BLOCK_MASS = 1000f;     //持っているブロックのMass
    Vector3 cableLockPositon;
    CableMoveBlock cableMoveBlock;
    // Use this for initialization
    void Start()
    {
        electric = Resources.Load<GameObject>("GameObject/particle/Electric");
        playerMove = playerCharacter.GetComponent<PlayerMove>();
        cablePositionDefault.transform.position = cablePosition.transform.position;
        ScaleChange(MIN_SCALE);
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        xBox = gameController.GetComponent<XBox>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;

        //ケーブルが伸び切るかオブジェクトに当たった時
        if (usbRange == MAX_SCALE && flag)
        {
            //フラグをfalseにする
            flag = false;
        }

        //ケーブルが縮み切った時
        if (!flag && usbRange == MIN_SCALE)
        {
            bool cFlag = false;
            //子オブジェクトにCableBlockがなければケーブルを消去する
            foreach (Transform child in cablePosition.transform)
            {
                if (child.gameObject.tag == "CableMoveBlock")
                {
                    cableFlag = true;
                    cFlag = true;
                }
            }
            if (!cFlag)
            {
                DestoryCable();
            }
        }

        //USBが縮んでいるとき
        if (!flag)
        {
            bool downFlag = false;
            foreach (Transform child in cablePosition.transform)
            {
                //子オブジェクトのケーブルブロックを探す
                if (child.gameObject.name == "CableMoveBlock")
                {
                    foreach (Transform child2 in child.transform)
                    {
                        //子オブジェクトのケーブルグラウンドを探す
                        if (child2.gameObject.tag == "CableGround")
                        {
                            float range = 0.5f;
                            //下にブロックが行っても大丈夫な状態ならケーブルの角度を変更する
                            if (Physics.Raycast(child2.transform.position + new Vector3(0f, range, 0f) + (transform.forward * range), -Vector3.up, 0.5f, blockMask + LayerMask.GetMask("Enemy")))
                            {
                                downFlag = true;
                            }
                        }
                    }
                }
            }
            if (chilledBlock != null)
            {
                //USBコードを正面を向けるようにする
                targetAngleRotateY(transform.gameObject, cablePositionDefault.transform.eulerAngles, CABLE_ROTATE_SPEED);
                if (!downFlag)
                {
                    targetAngleRotateX(transform.gameObject, cablePositionDefault.transform.eulerAngles, CABLE_ROTATE_SPEED);
                }
            }
        }
        //USBハブも正面に向ける
        if (!cableLock)
        {
            targetAngleRotateY(cablePosition, transform.eulerAngles, USB_ROTATE_SPEED);
        }

        //ケーブルの長さ変更
        if (!cableFlag)
        {
            //USBケーブルの長さ変更
            usbRange += SPEED * Time.deltaTime * BoolToInt(flag);
            //最大値や最小値を超えないように
            usbRange = Mathf.Clamp(usbRange, MIN_SCALE, MAX_SCALE);
        }

        if (flag)
        {
            RaycastHit hit;
            //レイを飛ばしてCableBlockがあるなら
            if (Physics.Raycast(transform.position, transform.forward,
                out hit, MAX_SCALE, cableMask))
            {
                GameObject hitOb = hit.collider.transform.parent.gameObject;

                foreach (Transform child in hitOb.transform)
                {
                    //子オブジェクトのケーブルブロックを探す
                    if (child.gameObject.tag == "CablePositon")
                    {
                        //角度の変更
                        transform.LookAt(child.transform.position);

                        //USBの角度の適応
                        cablePosition.transform.eulerAngles = transform.eulerAngles;
                        //ケーブルがUsbBoxに当たった時(通り過ぎた時)
                        if ((cablePosition.transform.position - (transform.position +
                            transform.forward * usbRange)).magnitude >
                            (cablePosition.transform.position - child.transform.position).magnitude)
                        {
                            if (!AboutFloat(transform.eulerAngles.y, child.transform.eulerAngles.y, 90f))
                            {
                                DestoryFlag();
                                UsbRangeNormal();
                                break;
                            }

                            gameController.SeSoundLoad(AudioController.Se.Player_UsbSet);
                            //USBの角度変更
                            transform.eulerAngles = child.eulerAngles;
                            //場所変更
                            cablePosition.transform.position = child.transform.position;
                            //Usbを指すための角度変更
                            cablePosition.transform.eulerAngles = child.eulerAngles;

                            if (hitOb.tag == "CableMoveBlock")
                            {
                                cableMoveBlock = hitOb.gameObject.GetComponent<CableMoveBlock>();
                                playerMove.cableFlag = false;
                                //子オブジェクトに設定
                                hitOb.transform.parent = cablePosition.transform;
                                flag = false;
                                //hitOb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                foreach (Transform child2 in hitOb.transform)
                                {
                                    child2.gameObject.layer = LayerMask.NameToLayer("Default");
                                }
                                hitOb.layer = LayerMask.NameToLayer("Default");
                                chilledBlock = hitOb;
                                //リジッドボディの削除
                                Destroy(hitOb.GetComponent<Rigidbody>());
                            }
                            else if (hitOb.tag == "CableSwitchBlock" || hitOb.tag == "ReloadBlock")
                            {
                                buttonUi.SetActive(true);
                                childPort = hitOb;
                                portBlockScript = childPort.GetComponent<PortBlock>();
                                ChildGimmickSearch(gimmickParent.transform, portBlockScript.portName);
                                cableFlag = true;
                                cableLockPositon = cablePosition.transform.position;
                                cableLock = true;
                                flag = false;
                            }
                        }
                        else
                        {
                            //USBを伸ばす
                            UsbRangeNormal();
                        }
                    }
                }
            }
            else
            {
                UsbRangeNormal();
            }
        }
        else
        {
            UsbRangeNormal();
        }


        //ケーブルを持っているときの上移動
        if (chilledBlock != null)
        {
            foreach (Transform child in cablePosition.transform)
            {
                //子オブジェクトのケーブルブロックを探す
                if (child.gameObject.name == "PortBlock")
                {
                    BlockUpDown(child.gameObject);
                }
            }
        }

        //スイッチに当たっているとき
        if (cableLock)
        {
            //電源を入れる処理---------------------------------------
            if (xBox.ButtonDown(XBox.Str.A))
            {
                //電源の切り替え
                pawerFlag = !pawerFlag;
                //時間のリセット
                timer = 0f;
            }

            if (pawerFlag)
            {
                PortBlock pB = childPort.GetComponent<PortBlock>();

                //変更する処理
                if (TimerCount(pB.Timer, ref timer))
                {
                    ElectricCreate(childPort);

                    gameController.SeSoundLoad(AudioController.Se.Player_UsbPower);
                    if (childPort.name == "ReloadPort")
                    {
                        playerCharacter.GetComponent<PlayerMove>().Reload();
                    }

                    for (int i = 0; i < childGimmick.Count; i++)
                    {
                        if (childGimmick[i].GetComponent<SwitchFlag>() != null)
                        {
                            SwitchFlag sF = childGimmick[i].GetComponent<SwitchFlag>();
                            sF.flag = !sF.flag;
                        }

                        if (childGimmick[i].GetComponent<SwitchBomb>() != null)
                        {
                            SwitchBomb sB = childGimmick[i].GetComponent<SwitchBomb>();
                            //入っていればカウントを進め時間が来れば発射する
                            if (sB.bulletNum > 0)
                            {
                                sB.bulletNum--;
                                //生成
                                GameObject bulletOb = Instantiate(sB.bullet);

                                //場所と角度の設定
                                foreach (Transform child in childGimmick[i].transform)
                                {
                                    if (child.transform.name == "BulletPositon")
                                    {
                                        bulletOb.transform.position = child.transform.position;
                                        bulletOb.transform.eulerAngles = child.transform.eulerAngles;
                                    }
                                }
                            }
                        }
                    }
                    pawerFlag = false;
                }
            }
            for (int i = 0; i < childGimmick.Count; i++)
            {
                //扇風機を回転させる処理-------------------------------------------------------
                if (childGimmick[i].name == "Wind")
                {
                    //場所と角度の設定
                    foreach (Transform child in childGimmick[i].transform)
                    {
                        //扇風機の羽の部分
                        if (child.transform.tag == "Wing")
                        {
                            Vector3 angle = child.gameObject.transform.eulerAngles;
                            Vector2 rotate = new Vector2(Input.GetAxis((XBox.AxisStr.LeftButtonUp).ToString()), Input.GetAxis((XBox.AxisStr.LeftButtonRight).ToString())).normalized * 30f * Time.deltaTime;
                            angle += new Vector3(rotate.x, rotate.y, 0F);
                            if (angle.x > 0 && angle.x < 90)
                                angle.x = 0f;
                            if (angle.x < -90)
                                angle.x = -90f;
                            child.gameObject.transform.eulerAngles = angle;
                            //場所と角度の設定
                        }
                    }
                }
            }
            UsbRangeNormal();
            cablePosition.transform.position = cableLockPositon;
            //ケーブルの長さを算出し、限界の長さを超えていたら消去
            if ((cableLockPositon - transform.position).magnitude > MAX_SCALE)
            {
                DestoryFlag();
                usbRange = MAX_SCALE;
            }

            //大きく動いたらケーブルを離す
            if (Mathf.Abs(xBox.AxisGet(XBox.AxisStr.LeftJoyRight)) > 0.8f ||
                Mathf.Abs(xBox.AxisGet(XBox.AxisStr.LeftJoyUp)) > 0.8f)
            {
                DestoryFlag();
            }
        }

        //ケーブルの向きの更新
        transform.LookAt(cablePosition.transform.position);
        //ケーブルの長さの更新
        ScaleChange(Vector3.Distance(cablePosition.transform.position, transform.position));

        //ケーブルを手放す処理
        if (chilledBlock != null && usbRange == MIN_SCALE)
        {
            if (playerMove.isGround ||
                !cableMoveBlock.isGround)
            {
                blockTimer = 0f;
            }
            else
            {
                if (TimerCount(BLOCK_TIMER_MAX, ref blockTimer))
                {
                    DestoryFlag();
                    blockTimer = 0f;
                }
            }
        }
    }

    //透視用
    void LayerAndMaterial(GameObject go)
    {
        if (go.GetComponent<ParticleSystem>() != null)
            return;

        foreach (Transform child in go.transform)
        {
            LayerAndMaterial(child.gameObject);
        }
        logLayer.Add(go.layer);

        if (go.layer == LayerMask.NameToLayer("Wind"))
        {
            go.layer = LayerMask.NameToLayer("VolcanoTransparent");
        }
        else
        {
            go.layer = LayerMask.NameToLayer("Transparent");
        }

        if (go.GetComponent<Renderer>() != null)
        {
            Renderer rend = go.GetComponent<Renderer>();
            logMaterial.Add(rend.sharedMaterial);
            rend.material.shader = Shader.Find("Transparent");
        }
    }

    //透視用
    void LayerAndMaterialEnd(GameObject go)
    {
        if (go.GetComponent<ParticleSystem>() != null)
            return;

        foreach (Transform child in go.transform)
        {
            LayerAndMaterialEnd(child.gameObject);
        }

        go.layer = logLayer[0];
        logLayer.RemoveAt(0);
        if (go.GetComponent<Renderer>() != null)
        {
            Renderer rend = go.GetComponent<Renderer>();
            rend.sharedMaterial = logMaterial[0];
            logMaterial.RemoveAt(0);
        }
    }

    void ChildGimmickSearch(Transform trans, string str)
    {
        foreach (Transform child in trans)
        {
            if (child.gameObject.name == "Port" + str)
            {
                LayerAndMaterial(child.gameObject);
                ChildGimmickAdd(child);
            }
            ChildGimmickSearch(child, str);
        }
    }

    void ChildGimmickSearchEnd(Transform trans, string str)
    {
        foreach (Transform child in trans)
        {
            if (child.gameObject.name == "Port" + str)
            {
                LayerAndMaterialEnd(child.gameObject);
            }
            ChildGimmickSearchEnd(child, str);
        }
    }

    void ChildGimmickAdd(Transform trans)
    {
        foreach (Transform child in trans)
        {
            if (child.gameObject.tag == "Gimmick")
            {
                if (child.gameObject.GetComponent<Wind>() != null)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("GameObject/ButtonWindUi"));
                    go.GetComponent<ButtonUiWind>().wind = child.gameObject;
                    windUi.Add(go);
                }

                childGimmick.Add(child.gameObject);
            }
            ChildGimmickAdd(child);
        }
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

    //USBの長さ変更
    void UsbRangeNormal()
    {
        cablePosition.transform.position =
                                transform.position + transform.forward * usbRange;
    }

    //オブジェクトの長さ変更
    void ScaleChange(float sacleZ)
    {
        transform.gameObject.transform.localScale
            = new Vector3(transform.gameObject.transform.localScale.x,
            transform.gameObject.transform.localScale.y,
            sacleZ);
    }

    //ブロックの上下
    void BlockUpDown(GameObject child)
    {
        RaycastHit hit;
        bool heightFlag = false;
        float range = 0.5f;
        float height = 0.0f;
        float scaleX = child.transform.localScale.x;
        float scaleZ = child.transform.localScale.z;

        //4つの角からレイをうつ
        for (int i = 0; i < 4; i++)
        {
            //子オブジェクトの地面の少し斜め前からレイを飛ばして地面に当たると上に行く
            BlockCast(child, range,
                BlockPosition(child, i < 2 ? scaleX : 0f, i % 2 == 0 ? scaleZ : 0f), ref heightFlag, ref height, out hit);
        }
        if (heightFlag)
        {
            transform.LookAt(cablePosition.transform.position + new Vector3(0f, height, 0f));
            UsbRangeNormal();
        }
    }

    //ブロック上下用
    void BlockCast(GameObject gameObject, float range, Vector3 position, ref bool flag, ref float height, out RaycastHit hit)
    {
        Physics.Raycast(gameObject.transform.position + new Vector3(0f, range, 0f)
                        + (gameObject.transform.up * (range)) + position, -Vector3.up,
                        out hit, 1f, blockMask);
        if (height < hit.point.y - gameObject.transform.position.y)
        {
            flag = true;
            height = hit.point.y - gameObject.transform.position.y;
        }
    }

    //ブロックの角
    Vector3 BlockPosition(GameObject gameObject, float xRange, float zRange)
    {
        return xRange * -gameObject.transform.right + zRange * gameObject.transform.up;
    }

    //初期化
    public void DefaultLoad()
    {
        windUi = new List<GameObject>();
        logLayer = new List<LayerMask>();
        logMaterial = new List<Material>();
        childGimmick = new List<GameObject>();
        cablePosition.transform.eulerAngles = transform.eulerAngles;
        cablePosition.transform.position = cablePositionDefault.transform.position;
        flag = true;
        cableFlag = false;
        cableLock = false;
        pawerFlag = false;
        chilledBlock = null;
        childPort = null;
        usbRange = MIN_SCALE;
        ScaleChange(MIN_SCALE);
    }

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

    //Centerの向いているへMoveObをSpeed(360°回る時間)の速さで向く。Verocityによって向きを変える
    //正面0 右90 後ろ180　左270
    void targetAngleRotateY(GameObject MoveOb, Vector3 Center, float Speed)
    {
        float angleY;
        bool ine;
        ine = Convert.ToBoolean((Mathf.DeltaAngle(MoveOb.transform.eulerAngles.y, Center.y) /
                                       Mathf.Abs(Mathf.DeltaAngle(MoveOb.transform.eulerAngles.y, Center.y))) + 1);

        angleY = MoveOb.transform.eulerAngles.y;
        angleY += 360f / (BoolToInt(ine) * Speed) * Time.deltaTime;

        if (BoolToInt(ine) * Mathf.DeltaAngle(angleY, Center.y) < 0f)
        {
            angleY = Center.y;
        }

        MoveOb.transform.eulerAngles = new Vector3
            (MoveOb.transform.eulerAngles.x,
            angleY,
            MoveOb.transform.eulerAngles.z);
    }

    //Centerの向いているへMoveObをSpeed(360°回る時間)の速さで向く。Verocityによって向きを変える
    //正面0 右90 後ろ180　左270
    void targetAngleRotateX(GameObject MoveOb, Vector3 Center, float Speed)
    {
        float angleY;
        bool ine;
        ine = Convert.ToBoolean((Mathf.DeltaAngle(MoveOb.transform.eulerAngles.x, Center.x) /
                                       Mathf.Abs(Mathf.DeltaAngle(MoveOb.transform.eulerAngles.x, Center.x))) + 1);

        angleY = MoveOb.transform.eulerAngles.x;
        angleY += 360f / (BoolToInt(ine) * Speed) * Time.deltaTime;

        if (BoolToInt(ine) * Mathf.DeltaAngle(angleY, Center.x) < 0f)
        {
            angleY = Center.x;
        }

        MoveOb.transform.eulerAngles = new Vector3
            (angleY,
            MoveOb.transform.eulerAngles.y,
            MoveOb.transform.eulerAngles.z);
    }

    //trueを+1 falseを-1として返す
    int BoolToInt(bool Inequality)
    {
        return (Convert.ToInt32(Inequality) * 2) - 1;
    }

    //ケーブルを削除し始めるフラグ
    public void DestoryFlag()
    {
        flag = false;
        foreach (Transform child in cablePosition.transform)
        {
            //ケーブルブロックの子要素をすべて開放
            if (child.transform.tag == "CableMoveBlock")
            {
                foreach (Transform child2 in child.transform)
                {
                    child2.gameObject.layer = LayerMask.NameToLayer("CableBlock");
                }
                child.gameObject.layer = LayerMask.NameToLayer("Block");
                child.gameObject.AddComponent<Rigidbody>();
                Rigidbody rigidbody = child.GetComponent<Rigidbody>();
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                rigidbody.mass = BLOCK_MASS;
                rigidbody.useGravity = false;
                child.parent = gameController.instansStage.transform;
            }
        }

        if (childPort != null)
        {
            ChildGimmickSearchEnd(gimmickParent.transform, portBlockScript.portName);
        }

        if (buttonUi.activeSelf)
        {
            buttonUi.SetActive(false);
        }

        while (windUi.Count != 0)
        {
            Destroy(windUi[0]);
            windUi.RemoveAt(0);
        }

        cableFlag = false;
        cableLock = false;
        childGimmick = new List<GameObject>();
        chilledBlock = null;
        childPort = null;
    }

    //ケーブル削除の処理
    public void DestoryCable()
    {
        timer = 0f;
        playerMove.cableFlag = false;
        cablePosition.SetActive(false);
        //アクティブを切る
        gameObject.SetActive(false);
    }

    //何かに当たった時
    void OnTriggerEnter(Collider col)
    {
        //当たった側のトリガーがオンなら処理をやめる
        if (col.isTrigger)
            return;

        foreach (Transform child in cablePosition.transform)
        {
            if (child.tag == "CableMoveBlock" && col.gameObject == child.gameObject)
            {
                return;
            }
        }

        DestoryFlag();
    }

    bool AboutFloat(float i, float j, float about)
    {
        float k = Mathf.DeltaAngle(i, j);
        return k < about && k > -about;
    }

    //スイッチ起動
    void ElectricCreate(GameObject port)
    {
        GameObject go = Instantiate(electric);
        go.transform.position = port.transform.position + port.transform.forward * 0.5f + -port.transform.right * 0.5f;
        go.transform.eulerAngles = new Vector3(-port.transform.eulerAngles.x + 90f, port.transform.eulerAngles.z, -port.transform.eulerAngles.y);
    }
}
