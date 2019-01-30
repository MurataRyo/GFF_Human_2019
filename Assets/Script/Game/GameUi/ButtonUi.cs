    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUi : MonoBehaviour {
    GameObject cameraOb;
    [HideInInspector] public Cable cable;
	// Use this for initialization
	void Start () {
        cameraOb = GameObject.FindGameObjectWithTag("Camera").transform.Find("Main Camera").gameObject;

        foreach(Transform child in GameObject.FindGameObjectWithTag("Player").transform)
        {
            if(child.gameObject.tag == "Cable")
            {
                cable = child.gameObject.GetComponent<Cable>();
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        PositionChenge();
        transform.LookAt(cameraOb.transform.position);
	}

    public virtual void PositionChenge()
    {

    }
}
