using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : BgmGimmick
{
    [HideInInspector] public GameObject windParticle;
    [HideInInspector] public GameObject windParticleInstance;
    [HideInInspector] public GameObject wind;
    [HideInInspector] public float windPower;
    public float windRange;

    [HideInInspector]public CapsuleCollider capCol;

    bool flagLog = false;

    public override void LowStart()
    {
        if(windRange < 3f)
        {
            windRange = 3f;
        }

        windParticleInstance = Resources.Load<GameObject>("GameObject/Particle/Wind");

        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "Wing")
            {
                wind = child.gameObject;
                windPower = wind.GetComponent<VelosityBlock>().power;
                col = wind.GetComponent<Collider>();
            }
        }
        audioRangeMax = 10f;
    }

    public override void LateUpdate()
    {
        ParticleC();

        AudioC();
    }

    public void ParticleC()
    {
        if (flag != flagLog)
        {
            if (flag)
            {
                CreateCapsule();

                ParticleSystem.MainModule ps = windParticleInstance.GetComponent<ParticleSystem>().main;
                ps.startSpeedMultiplier = windPower;
                float t = windRange / windPower;
                ps.startLifetime = new ParticleSystem.MinMaxCurve(t - 0.5f, t + 0.5f);

                ParticleSystem.EmissionModule em = windParticleInstance.GetComponent<ParticleSystem>().emission;
                em.rateOverTimeMultiplier = windPower * 1.5f;

                windParticle = Instantiate(windParticleInstance);
                windParticle.transform.parent = wind.transform;
                windParticle.transform.position = wind.transform.position;
                windParticle.transform.eulerAngles = wind.transform.eulerAngles;
            }
            else
            {
                Destroy(capCol);
                Destroy(windParticle);
            }
        }
        flagLog = flag;
    }

    public void CreateCapsule()
    {
        capCol = wind.gameObject.AddComponent<CapsuleCollider>();
        col = capCol.GetComponent<Collider>();
        capCol.isTrigger = true;
        capCol.direction = 2;       //Z方向
        capCol.radius = 1.5f;
        capCol.height = 3f;
        CapColCenter(capCol);
        StartCoroutine(ColChange());
    }

    IEnumerator ColChange()
    {
        while (capCol != null && capCol.height != windRange)
        {
            capCol.height = Mathf.MoveTowards(capCol.height, windRange, windPower * Time.deltaTime);
            CapColCenter(capCol);
            yield return null;
        }
        yield break;
    }

    public void CapColCenter(CapsuleCollider col)
    {
        col.center = new Vector3(0f, 0f, col.height / 2 - col.radius);
    }

    void AudioC()
    {
        rangeFlag = RangeIf(col, out range);

        if (rangeFlag != rangeFlagLog)
        {
            if (rangeFlag)
            {
                gameController.BgmSoundLoad(AudioController.Bgm.Gimmick_Wind, out source);
                source.Play();
            }
            else
            {
                Destroy(source.gameObject);
            }
        }

        if (rangeFlag)
            VolumeChange();

        rangeFlagLog = rangeFlag;
    }
}
