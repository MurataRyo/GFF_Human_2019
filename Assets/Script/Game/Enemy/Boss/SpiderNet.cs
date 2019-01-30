using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpiderNet : MonoBehaviour {
    [SerializeField]
    GameObject bugPrefab;   //投げるオブジェクト
    GameObject netPos;      //糸の先端
    float defaultScaleZ;        //初期の大きさ
    const float START_SCALE = 0.1f;
    const float MIN_SCALE = 2f; 
    float netRange;             //現在の糸の長さ
    const float SPEED = 5.0f;  //1秒間に延びる長さ(倍率)
    bool flag = true;                   //ケーブルの伸縮する方向(trueが伸びる。falseが縮む)
	// Use this for initialization
	void Start () {
        foreach(Transform child in transform.parent.transform.parent)
        {
            if(child.gameObject.name == "NetPos")
            {
                netPos = child.gameObject;
            }
        }
        foreach (Transform child in transform)
        {
            {
                defaultScaleZ = child.transform.localScale.z;
            }
        }
        netRange = START_SCALE;
        ScaleChange(netRange);
    }
	
	// Update is called once per frame
	void Update () {
        netRange += BoolToInt(flag) * SPEED * Time.deltaTime;

        ScaleChange(netRange / defaultScaleZ);
        NetRangeNormal();
        if(!flag && (netPos.transform.position - transform.position).magnitude < MIN_SCALE)
        {
            //ブロックを持っている状態にする
            transform.parent.transform.parent.GetComponent<SpiderMove>().BugBlockFlag = true;
            //このスクリプトを削除
            Destroy(this);
        }
    }

    //糸の長さ変更
    void NetRangeNormal()
    {
        netPos.transform.position =
                                transform.position + transform.forward * netRange * 2;
    }

    //オブジェクトの長さ変更
    void ScaleChange(float sacleZ)
    {
        transform.localScale
            = new Vector3(transform.localScale.x,
            transform.localScale.y,
            sacleZ);
    }

    //trueを+1 falseを-1として返す
    int BoolToInt(bool Inequality)
    {
        return (Convert.ToInt32(Inequality) * 2) - 1;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "BugBlock" || !flag)
            return;

        //バグの生成
        GameObject go = Instantiate(bugPrefab);
        go.transform.position = netPos.transform.position;
        go.transform.eulerAngles = netPos.transform.eulerAngles;
        go.transform.parent = netPos.transform;
        flag = false;
    }
}
