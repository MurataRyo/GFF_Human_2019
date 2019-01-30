using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AlphaBlock : MonoBehaviour
{
    Renderer rend;
    bool alphaFlag = false;
    const float SPEED = 2f;
    const float STOP_TIME = 5f;
    const float MAX_ALPHA = 1.0f;
    const float MIN_ALPHA = 0.1f;
    float timer;
    float alphaColor;
    // Use this for initialization
    void Start()
    {
        startFlag();
    }
    //初期化
    void startFlag()
    {
        timer = UnityEngine.Random.Range(0f, STOP_TIME);
        rend = GetComponent<Renderer>();
        AlphaChange(alphaColor);
    }

    // Update is called once per frame
    void Update()
    {
        AlphaChange();
        Collider col = GetComponent<Collider>();
        if (!alphaFlag && alphaColor > MIN_ALPHA || alphaFlag && alphaColor == MAX_ALPHA)
        {
            // Trigger OFF 
            col.enabled = true;
        }
        else
        {
            // Trigger ON 
            col.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            startFlag();
        }
    }
    //Alphaの変更
    void AlphaChange()
    {
        //Alphaの上昇と下降
        alphaColor += BoolToInt(alphaFlag) / SPEED * Time.deltaTime;
        //Alphaがはみ出ないように
        alphaColor = Mathf.Clamp(alphaColor, 0f, MAX_ALPHA);

        if (TimeIf(SPEED, STOP_TIME))
        {
            alphaFlag = !alphaFlag;
        }

        //反映
        if (!alphaFlag)
        {
            if (alphaColor > MIN_ALPHA)
            {
                //Alpha減少中
                AlphaChange(alphaColor);
            }
            else
            {
                //Alpha上昇中
                AlphaChange(0f);
            }
        }
        else
        {
            if (alphaColor != 1)
            {
                //Alpha減少中（消滅）
                AlphaChange(0f);
            }
            else
            {
                AlphaChange(1f);
            }
        }
    }

    //時間の計算
    bool TimeIf(float time, float time2)
    {
        time += time2;
        timer += Time.deltaTime;
        if (time < timer)
        {
            timer = 0f;
            return true;
        }
        return false;

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