using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beltconveyor : BgmGimmick
{
    AudioController.Bgm audioSource = AudioController.Bgm.Gimmick_Beltconveyor;
    Conveyor conveyor;

    public override void LowStart()
    {
        volumeMax = 1f;

        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "ConveyorChild")
            {
                conveyor = child.GetComponent<Conveyor>();
            }
        }
        col = GetComponent<Collider>();
        audioRangeMax = 10f;
    }

    public override void LateUpdate()
    {
        range = (col.ClosestPointOnBounds(player.transform.position) - player.transform.position).magnitude;

        float volume = (audioRangeMax - range) / audioRangeMax * volumeMax *
            Mathf.Abs(1 - (conveyor.speedMax - conveyor.speed) / conveyor.speedMax);

        rangeFlag = volume > 0f;

        if (rangeFlag != rangeFlagLog)
        {
            if (rangeFlag)
            {
                gameController.BgmSoundLoad(audioSource, out source);
                source.Play();
            }
            else
            {
                Destroy(source.gameObject);
            }
        }

        if (rangeFlag)
            source.volume = volume;

        rangeFlagLog = rangeFlag;
    }
}