using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSand : SwitchFlag
{
    //流砂関連
    Vector3 centerPos;
    float speed = 3f;
    float about = 0.1f;

    //UI関連
    GameObject sandUi;
    QuickSandUi sandUiScript;
    // Use this for initialization
    void Start()
    {
        foreach (Transform child in GameObject.FindGameObjectWithTag("Canvas").transform)
        {
            if (child.gameObject.name == "SandUi")
            {
                sandUi = child.gameObject;
            }
        }
        sandUiScript = sandUi.GetComponent<QuickSandUi>();
        centerPos = transform.position + (transform.localScale / 2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag != "Player")
            return;
        PlayerMove playerMove = col.gameObject.GetComponent<PlayerMove>();
        playerMove.stopPlayer = true;
        OnTrigger(col.gameObject, ref playerMove.objectVelosity, ref playerMove.gravity);
    }

    public void OnTrigger(GameObject go, ref Vector3 objectVelosity, ref float gravity)
    {
        if (AboutFloat(go.transform.position.x, centerPos.x) && AboutFloat(go.transform.position.z, centerPos.z))
        {
            if (!sandUiScript.activeFlag)
            {
                sandUi.SetActive(true);
                sandUiScript.SandUi(flag);
            }
        }
        gravity = -9.81f * Time.deltaTime;
        objectVelosity = (centerPos - go.transform.position).normalized * speed;
    }

    bool AboutFloat(float x, float y)
    {
        if (x + about >= y &&
            x - about <= y)
        {
            return true;
        }
        return false;
    }
}
