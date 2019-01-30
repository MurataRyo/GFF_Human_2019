using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlock : SwitchFlag {
    [SerializeField]
    float CreateIntervalMin;

    [SerializeField]
    float CreateIntervalMax;

    GameObject stone;

    void Start()
    {
        stone = Resources.Load<GameObject>("GameObject/Stone");
        StartCoroutine(Create());
    }

    IEnumerator Create()
    {
        while(true)
        {
            while(!flag)
            {
                yield return null;
            }
            CreateStone();
            yield return new WaitForSeconds(Random.Range(CreateIntervalMin,CreateIntervalMax));
        }
    }

    void CreateStone()
    {
        GameObject go = stone;
        Instantiate(stone);
        go.transform.localScale = transform.localScale;
        go.transform.position = transform.position + ((transform.forward + transform.right) * transform.localScale.x / 2);
        go.transform.eulerAngles = transform.eulerAngles;
    }
}
