using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAntMove : AntMove {

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMove>().Damage();
        }
    }

}
