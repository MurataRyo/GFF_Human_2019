using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AlphaGimmick : MonoBehaviour {

    GameObject player;          //プレイヤーの場所取得用
    Renderer rend;              //色変更用
    [SerializeField]
          float seeRange;       //みえる範囲
    float alphaColor = 0f;      //透明度（０～１）
    const float SPEED = 0.5f;   //値の変化するスピード
    const float 
        MAX_ALPHA = 0.3f;       //アルファ値の最大値
    bool alphaFlag = false;     //透明度の変化する方向

    // Use this for initialization
    void Start () {
        //いろいろ取得
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (Transform child in player.transform)
        {
            if (child.gameObject.name == "PlayerCharacter")
            {
                player = child.gameObject;
            }
        }

        rend = GetComponent<Renderer>();
        AlphaChange(alphaColor);
    }
	
	// Update is called once per frame
	void Update () {

        //Alphaの上昇と下降
        alphaColor += BoolToInt(alphaFlag) / SPEED * MAX_ALPHA * Time.deltaTime;
        //Alphaがはみ出ないように
        alphaColor = Mathf.Clamp(alphaColor, 0f, MAX_ALPHA);

        //みえるようになる範囲ならtrueそうでないならfalseを返す
        if (Mathf.Abs((transform.position - player.transform.position).magnitude) < seeRange)
        {
            alphaFlag = true;
        }
        else
        {
            alphaFlag = false;
        }
        //透明度の更新
        AlphaChange(alphaColor);
    }

    //trueを+1 falseを-1として返す
    int BoolToInt(bool i)
    {
        return (Convert.ToInt32(i) * 2) - 1;
    }

    void AlphaChange(float alphaCollar)
    {
        rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, alphaCollar);
    }
}
