using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapNet : MonoBehaviour
{
    Rigidbody rBody;
    const float SPEED = 15f;
    [SerializeField]
    GameObject trapNetGround;
    const float DESTROY_TIME = 10f;
    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.velocity = transform.forward * SPEED;
    }

    void OnCollisionStay(Collision col)
    {
        //ボスなら実行しない
        if (col.gameObject.tag == "Ground")
        {
            GameObject go = Instantiate(trapNetGround);
            go.transform.position = transform.position;
            Destroy(go, DESTROY_TIME);
            Destroy(gameObject);
        }
    }
}
