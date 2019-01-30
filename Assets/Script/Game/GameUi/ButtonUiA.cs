using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUiA : ButtonUi {

    public override void PositionChenge()
    {
        transform.position = cable.childPort.transform.position +
            cable.childPort.transform.forward + -cable.childPort.transform.right / 2 + new Vector3(0f, cable.childPort.transform.localScale.y / 2f, 0f);
    }
}
