using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed;
    [SerializeField]
    GameObject Explositon;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.isTrigger)
            return;

        GameObject go =  Instantiate(Explositon);
        Destroy(go, 3f);
        go.transform.position = transform.position;
        Destroy(gameObject);
    }
}
