using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    Animator anim;
    [HideInInspector] public float speedMax;
    [HideInInspector]
    public float speed;
    SwitchFlag swFlag;
    const float STOP_TIME = 2f;
    const float MAX_TIME = 2f;
    // Use this for initialization
    void Start()
    {
        swFlag = transform.parent.GetComponent<SwitchFlag>();
        speedMax = transform.parent.GetComponent<VelosityBlock>().power;
        speed = swFlag.flag ? speedMax : 0f;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        speedMax = transform.parent.GetComponent<VelosityBlock>().power;
        if (swFlag.flag)
        {
            speed += speedMax / MAX_TIME * Time.deltaTime;
        }
        else
        {
            speed -= speedMax / STOP_TIME * Time.deltaTime;
        }
        speed = Mathf.Clamp(speed, 0f, speedMax);
        anim.speed = speed;
    }
}
