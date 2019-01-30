using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpWind : Wind
{
    public override void LowStart()
    {
        if (windRange < 3f)
        {
            windRange = 3f;
        }
        flag = true; 

        windParticleInstance = Resources.Load<GameObject>("GameObject/Particle/Wind");
        wind = gameObject;
        windPower = wind.GetComponent<VelosityBlock>().power;
        
        audioRangeMax = 10f;

        CreateCapsule();
        capCol.height = windRange;
        CapColCenter(capCol);
    }
}
