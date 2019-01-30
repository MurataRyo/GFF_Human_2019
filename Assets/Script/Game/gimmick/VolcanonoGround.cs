using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanonoGround : MonoBehaviour
{
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMove>().TriggerVolcano();
        }
    }
}