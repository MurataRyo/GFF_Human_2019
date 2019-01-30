using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableMoveBlock : MonoBehaviour
{
    const float GRAVITY_SIZE = -9.81f;
    float gravity = 0f;
    public bool isGround = true;
    Vector3 velocity;


    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Rigidbody>())
        {
            Rigidbody rBody = GetComponent<Rigidbody>();
            if(isGround)
            {
                gravity = GRAVITY_SIZE * Time.deltaTime;
            }
            else
            {
                gravity += GRAVITY_SIZE * Time.deltaTime;
            }
            rBody.velocity = velocity + new Vector3(0f, gravity, 0f);
        }
    }

    void FixedUpdate()
    {
        isGround = false;
        velocity = Vector3.zero;
    }

    //接地判定
    public void IsGround(Collision col)
    {
        for (int i = 0; i < col.contacts.Length; i++)
        {
            if (col.contacts[i].normal.y > 0.5f)
            {
                isGround = true;
            }
        }
    }

    //当たっている時
    void OnCollisionStay(Collision col)
    {
        //接地判定
        IsGround(col);

        //当たったものの確認
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Block" || col.gameObject.tag == "CableBlock" || col.gameObject.tag == "CableSwitchBlock" 
            || col.gameObject.tag == "Boss" || col.gameObject.tag == "Net" || col.gameObject.tag == "Gimmick")
        {
            if (col.gameObject.GetComponent<VelosityBlock>() && !col.collider.isTrigger)
            {
                for (int i = 0; i < col.contacts.Length; i++)
                {
                    //プレイヤーがコンベヤーの上にいるとき
                    if (col.contacts[i].point.y <= transform.position.y + 0.1f)
                    {
                        //その力を返す
                        velocity = col.gameObject.GetComponent<VelosityBlock>().VelosityObject(ref gravity);
                        break;
                    }
                }
            }
        }
    }
}

