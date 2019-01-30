using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmGimmick : SwitchFlag
{
    [HideInInspector] public bool rangeFlag;
    [HideInInspector] public bool rangeFlagLog;
    [HideInInspector] public float audioRangeMax;
    [HideInInspector] public GameController gameController;
    [HideInInspector] public GameObject player;
    [HideInInspector] public AudioSource source;
    [HideInInspector] public Collider col;
    [HideInInspector] public float range;
    [HideInInspector] public float volumeMax = 1f;

    void Start()
    {
        rangeFlag = rangeFlagLog = false;
        gameController =
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        player =
            GameObject.FindGameObjectWithTag("Player");
        LowStart();
    }

    public virtual void LateUpdate()
    {

    }

    public virtual void LowStart()
    {

    }

    public bool RangeIf(Collider col, out float range)
    {
        range = 0f;

        if (!flag)
            return false;

        range = (col.ClosestPointOnBounds(player.transform.position) - player.transform.position).magnitude;

        return rangeFlag = range < audioRangeMax;
    }


    public virtual void VolumeChange()
    {
        source.volume = (audioRangeMax - range) / audioRangeMax * volumeMax;
    }
}
