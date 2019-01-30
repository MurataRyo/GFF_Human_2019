using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volcano : MonoBehaviour {
    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyMove>().DestoryEnemy();
        }
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMove>().TriggerVolcano();
            col.GetComponent<PlayerMove>().Damage();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMove>().ExitVolcano();
        }
    }
}