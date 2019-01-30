using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetGround : MonoBehaviour
{
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMove>().trapFlag = true;
        }
    }
}
