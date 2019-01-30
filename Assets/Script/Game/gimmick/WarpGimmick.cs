using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpGimmick : MonoBehaviour
{
    bool warpFlag = false;  //ワープが起動しているかのフラグ
    Vector3 warpPosition;   //ワープ地点保存用

    // Use this for initialization
    void Start()
    {
        foreach (Transform child in transform)
        {
            //子の名前がWarpEndのときにTrue
            if (child.name == "WarpEnd")
            {
                //WarpEndにEarpPositionを合わせる
                warpPosition = child.transform.position;
            }
        }
    }

    IEnumerator Warp(GameObject warpPlayer)
    {
        warpFlag = true;
        yield return new WaitForSeconds(3f);

        warpPlayer.transform.position = warpPosition;
        warpFlag = false;
        yield break;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!warpFlag && col.gameObject.tag == "Player" || !warpFlag && col.gameObject.tag == "Enemy")
        {
            StartCoroutine(Warp(col.gameObject));
        }
    }
}