using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bridge : SeGimmick {

    AudioController.Se audioSource = AudioController.Se.Gimmick_Bridge;

    public override void SePlay()
    {
        gameController.SeSoundLoad(audioSource,out audioObject);
    }
}
