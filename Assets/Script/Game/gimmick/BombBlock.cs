using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBlock : MonoBehaviour
{
    GameObject cable;           //ケーブルを入れる
    GameObject player;
    // Use this for initialization
    void Start()
    {
        //いろいろ取得
        //ケーブルを取得する
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (Transform child in player.transform)
        {
            if (child.gameObject.tag == "Cable")
            {
                cable = child.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider col)
    {
        //スイッチボムがなければ何もしない
        if (!col.GetComponent<SwitchBomb>())
            return;

        SwitchBomb sB = col.GetComponent<SwitchBomb>();

        //弾が装填状態なら何もしない
        if (sB.bulletNum >= sB.bulletNumMax)
            return;

        //弾を込める
        cable.GetComponent<Cable>().DestoryFlag();
        sB.bulletNum++;
        Destroy(gameObject);

    }
}
