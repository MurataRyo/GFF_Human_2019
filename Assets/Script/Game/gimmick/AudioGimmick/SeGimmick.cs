using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeGimmick : SwitchFlag
{
    [HideInInspector]public GameObject audioObject;
    [HideInInspector]public GameController gameController;
    bool logFlag;

    void Start()
    {
        gameController =
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        logFlag = flag;
        LowStart();
    }

	// Update is called once per frame
	void LateUpdate () {
		if(logFlag != flag)
        {
            if (audioObject)
            {
                Destroy(audioObject);
            }
            SePlay();
        }
        logFlag = flag;
	}

    public virtual void LowStart()
    {

    }

    public virtual void SePlay()
    {

    }
}
