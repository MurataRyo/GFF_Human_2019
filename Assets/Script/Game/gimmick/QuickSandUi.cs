using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuickSandUi : MonoBehaviour
{

    //色々
    GameObject player;
    PlayerMove playerScript;
    GameController gameController;

    const float SAND_CHANGE_HEIGHT = 1008f;
    const float SAND_UI_HEIGHT = 1080f * 3f;
    const float END_SPEED = 1f;

    [HideInInspector]
    public bool activeFlag = false;


    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerMove>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

    }

    public void SandUi(bool clearFlag)
    {
        activeFlag = true;
        transform.position = new Vector3(960f, -1080f, 0f);
        StartCoroutine(SandUiCor(clearFlag));
    }

    IEnumerator SandUiCor(bool clearFlag)
    {
        bool setStageFlag = false;
        do
        {
            yield return null;
            transform.position += new Vector3(0f, SAND_UI_HEIGHT / END_SPEED * Time.unscaledDeltaTime, 0f);

            //プレイヤーの移動や画面の繊維(シーン移動無し)
            if (!setStageFlag && transform.position.y > SAND_UI_HEIGHT - SAND_CHANGE_HEIGHT)
            {
                setStageFlag = true;
                playerScript.stopPlayer = false;
                //正解かどうか
                if (clearFlag)
                {
                    if((int)gameController.sandNumber +1 == Enum.GetValues(typeof(GameController.SandStageNumber)).Length)
                    {
                        player.GetComponent<PlayerMove>().StageClear();
                    }
                    else
                    {
                        gameController.ChangeStage();
                    }
                }
                else
                {
                    player.transform.position = gameController.startPosition;
                }
                // Time.timeScale = 0f;
            }

        }
        while (transform.position.y < SAND_UI_HEIGHT + 1080f);
        // Time.timeScale = 1f;
        activeFlag = false;

        gameObject.SetActive(false);
        yield break;
    }
}
