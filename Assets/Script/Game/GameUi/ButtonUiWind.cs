using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUiWind : ButtonUi {
    [HideInInspector]public GameObject wind;

    public override void PositionChenge()
    {
        transform.position = wind.transform.position +
          (wind.transform.forward * 1.5f + wind.transform.right * 1.5f + new Vector3(0f,3f, 0f)) * transform.localScale.x;
    }
}