using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour {
    Vector3 defaultPosition;    //初期位置
    [SerializeField]
            float speed;        //スピード
    [SerializeField]
            float Interval;     //間隔(秒)
	// Use this for initialization
	void Start () {
        defaultPosition = transform.position;   //初期位置を取得
        StartCoroutine(Move());
	}
	
	// Update is called once per frame
	void Update () {
        //移動
        transform.Translate(transform.forward * speed * Time.deltaTime);
    }

    IEnumerator Move()
    {
        while(true)
        {
            yield return new WaitForSeconds(Interval);  //間隔の時間だけここで止める
            foreach (Transform child in transform)
            {
                foreach (Transform child2 in child.transform)
                {
                   child2.parent = null;
                }
            }
            transform.position = defaultPosition;       //位置のリセット
        }
    }
}
