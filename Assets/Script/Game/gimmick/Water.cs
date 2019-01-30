using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    GameController gameController;
    void Start()
    {
        gameController = 
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            gameController.SeSoundLoad(AudioController.Se.Gimmick_Water);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMove>().TriggerWater();
        }
    }
}
