using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelosityBlock : MonoBehaviour
{
    public float power;        //与える力

    public Vector3 VelosityObject()
    {
        //ベルトコンベアーの場合か扇風機の時
        if (transform.name == "Beltconveyor")
        {
            foreach(Transform child in transform)
            {
                if(child.gameObject.tag == "ConveyorChild")
                {
                    //向きにそのまま力を返す
                    return transform.forward * child.GetComponent<Conveyor>().speed;
                }
            }
        }
        return Vector3.zero;
    }

    public Vector3 VelosityObject(ref float gravity)
    {
        //ベルトコンベアーの場合か扇風機の時
        if (transform.name == "Beltconveyor")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.tag == "ConveyorChild")
                {
                    //向きにそのまま力を返す
                    return transform.forward * child.GetComponent<Conveyor>().speed;
                }
            }
        }
        else if (transform.tag == "Wing")
        {
            gravity = 0f;
            //向きにそのまま力を返す
            return transform.forward * power;
        }
        return Vector3.zero;
    }

    public virtual void OnTriggerStay(Collider col)
    {
        //当たり判定がなければ終了
        if (col.isTrigger)
            return;
        //親にSwitch存在していてFlagがついていなければ起動しない
        if (transform.parent.GetComponent<SwitchFlag>() && !transform.parent.GetComponent<SwitchFlag>().flag)
            return;

        //障害物があれば動かない
        RaycastHit hit;
        if (Physics.Linecast(transform.position, col.gameObject.transform.position, out hit))
        {
            if (hit.collider.tag == "CableMoveBlock")
            {
                if (!hit.collider.isTrigger)
                {
                    return;
                }
            }
        }

        //当たったオブジェクトがプレイヤーか敵なら
        if (col.gameObject.tag == "Player")
        {
            PlayerMove pm = col.GetComponent<PlayerMove>();
            if (pm.stopPlayer)
            {
                return;
            }

            pm.WindTrigger(gameObject);
            pm.windFlag = false;
        }
        else if (col.gameObject.tag == "Enemy")
        {
            col.GetComponent<EnemyMove>().WindTrigger(gameObject);
            col.GetComponent<EnemyMove>().windFlag = true;
        }
    }

    public virtual void OnTriggerExit(Collider col)
    {
        //当たり判定がなければ終了
        if (col.isTrigger)
            return;
        //親にSwitch存在していてFlagがついていなければ起動しない
        if (transform.parent.GetComponent<SwitchFlag>() && !transform.parent.GetComponent<SwitchFlag>().flag)
            return;

        //風ギミックなら
        if (transform.tag == "Wing")
        {
            //当たったオブジェクトにVelosityBlockが入っていたなら
            if (col.gameObject.tag == "Player")
            {
                col.GetComponent<PlayerMove>().windVelosityLog =
                    transform.forward * power;
                col.GetComponent<PlayerMove>().windFlag = true;
            }
            if (col.gameObject.tag == "Enemy")
            {
                col.GetComponent<EnemyMove>().windVelosityLog =
                    transform.forward * power;
            }
        }
    }
}
