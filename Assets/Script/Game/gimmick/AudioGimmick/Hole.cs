using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : SeGimmick {
    AudioController.Se audioSource1 = AudioController.Se.Gimmick_HoleGround;
    AudioController.Se audioSource2 = AudioController.Se.Gimmick_HoleVacant;

    public override void SePlay()
    {
        gameController.SeSoundLoad(flag ? audioSource1 : audioSource2,out audioObject);
    }
}
