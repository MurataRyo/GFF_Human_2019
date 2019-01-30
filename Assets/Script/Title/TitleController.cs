using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [SerializeField] Image Mori;
    [SerializeField] Image Sabaku;
    [SerializeField] Image Kazan;
    [SerializeField] Image Tutorial;
    [SerializeField] Image Choice;
    int choiceStage = 1;    //選択中のステージ
    int choiceOkStage;      //選択可能なステージ数
    XBox xBox;
    [SerializeField] RenbanMovie renbanMovie;

    AudioController audioC;
    bool ngFlag = false;
    bool scrollFlag = false;
    bool selectFlag = false;
    bool tutorialChoice = false;

    void Start()
    {
        audioC = GetComponent<AudioController>();
        audioC.BgmSoundPlay(AudioController.Bgm.Title_Bgm);

        xBox = GetComponent<XBox>();
        //選択可能なステージの設定
        choiceOkStage = (int)GameController.clearNumber;
    }

    void Update()
    {
        if (!ngFlag)
        {
            if (scrollFlag)
            {
                renbanMovie.titleUi.transform.localPosition = Vector3.MoveTowards(renbanMovie.titleUi.transform.localPosition, new Vector3(0f, 540f, 0f), 2160f * Time.deltaTime);
            }

            if (renbanMovie.titleUi.transform.localPosition == new Vector3(0f, 540f, 0f))
            {
                selectFlag = true;
                scrollFlag = false;
            }

            //スタートボタン
            if (xBox.ButtonDown(XBox.Str.Start) ||
                xBox.ButtonDown(XBox.Str.A))
            {
                if (!scrollFlag)
                {
                    audioC.SeSoundPlay(AudioController.Se.Enter);
                }

                if (!selectFlag && !scrollFlag)
                {
                    scrollFlag = true;
                }
                else if (selectFlag)
                {
                    if (tutorialChoice)
                    {
                        choiceStage = 0;
                    }
                    GameController.stageNumber = (GameController.StageNumber)Enum.ToObject(typeof(GameController.StageNumber), choiceStage);
                    //ステージへ
                    LoadScene.sceneName = "Stage";
                    StartCoroutine(Load(audioC.clip));
                }
            }
            ChangeStage();
        }

        // ステージアイコンの描画処理---------------------------------------------------------
        if (choiceOkStage > 2)
        {
            Kazan.enabled = true;
            Mori.transform.localPosition = new Vector3(-600f, -810f, 0f);
            Sabaku.transform.localPosition = new Vector3(0f, -810f, 0f);
        }
        else
        {
            Kazan.enabled = false;
            Mori.transform.localPosition = new Vector3(-300f, -810f, 0f);
            Sabaku.transform.localPosition = new Vector3(300f, -810f, 0f);
        }
        if (choiceOkStage > 1)
        {
            Sabaku.enabled = true;
        }
        else
        {
            Sabaku.enabled = false;
            Mori.transform.localPosition = new Vector3(0f, -810f, 0f);
        }
        //----------------------------------------------------------------------------------

        // カーソルの移動
        if (tutorialChoice)
        {
            ChoicepositonUpdate(Tutorial);
        }
        else
        {
            if (choiceStage == 1)
            {
                ChoicepositonUpdate(Mori);
            }
            else if (choiceStage == 2)
            {
                ChoicepositonUpdate(Sabaku);
            }
            else if (choiceStage >= 3)
            {
                ChoicepositonUpdate(Kazan);
            }
        }
    }

    //選択UIの場所移動
    void ChoicepositonUpdate(Image map)
    {
        Choice.transform.localPosition =
                new Vector3(map.transform.localPosition.x - 240f, map.transform.localPosition.y - 40f, 0f);
    }

    //音が終わるのを待つ
    IEnumerator Load(AudioClip audioClip)
    {
        ngFlag = true;

        if (selectFlag)
        {
            yield return new WaitForSeconds(audioClip.length);
            SceneManager.LoadScene("Load");
        }
    }
    //選択変更
    void ChangeStage()
    {
        if (selectFlag)
        {
            if (!tutorialChoice)
            {
                if (xBox.ButtonDown(XBox.AxisStr.LeftButtonRight, false) ||
                    xBox.ButtonDown(XBox.AxisStr.LeftJoyRight, false))
                {
                    SelectAudio();
                    if (choiceStage == 1)
                    {
                        choiceStage = choiceOkStage;
                    }
                    else
                    {
                        choiceStage--;
                    }
                }
                if (xBox.ButtonDown(XBox.AxisStr.LeftButtonRight, true) ||
                    xBox.ButtonDown(XBox.AxisStr.LeftJoyRight, true))
                {
                    SelectAudio();
                    if (choiceStage >= choiceOkStage)
                    {
                        choiceStage = 1;
                    }
                    else
                    {
                        choiceStage++;
                    }
                }
            }

            if (xBox.ButtonDown(XBox.AxisStr.LeftButtonUp, true) ||
                xBox.ButtonDown(XBox.AxisStr.LeftButtonUp, false) ||
                xBox.ButtonDown(XBox.AxisStr.LeftJoyUp, true) ||
                xBox.ButtonDown(XBox.AxisStr.LeftJoyUp, false))
            {
                SelectAudio();
                tutorialChoice = !tutorialChoice;
            }
        }
    }

    void SelectAudio()
    {
        audioC.SeSoundPlay(AudioController.Se.Select);
    }
}
