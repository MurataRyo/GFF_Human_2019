using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationGimmick : MonoBehaviour {
    //アニメーション用
    bool upFlag = false;
    Animator animator;
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        animator.SetBool("Upflag", upFlag);
    }
	
	// Update is called once per frame
	void Update () {
        if(transform.parent.GetComponent<SwitchFlag>())
        {
            upFlag = transform.parent.GetComponent<SwitchFlag>().flag;
        }
        else if(transform.parent.transform.parent.GetComponent<SwitchFlag>())
        {
            upFlag = transform.parent.transform.parent.GetComponent<SwitchFlag>().flag;
        }
        
        animator.SetBool("Upflag", upFlag);
    }
}
