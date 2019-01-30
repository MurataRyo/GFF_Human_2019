using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpWindVelosity : VelosityBlock {

    public override void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            return;
        }
        base.OnTriggerStay(col);
    }

    public override void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            return;
        }
        base.OnTriggerExit(col);
    }
}
