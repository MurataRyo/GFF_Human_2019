using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttachEnemy : MonoBehaviour {

    Type type;

    // Use this for initialization
    void Start () {
        type = Type.GetType(gameObject.name + "Move");
        gameObject.AddComponent(type);
        Destroy(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
