using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //選択しているステージ--------------------------------
    //ステージ名を順番に記入
    public enum StageNumber
    {
        Stage0,
        Stage1,
        Stage2,
        Stage3
    }
    //現在のステージ(選択している)
    public static StageNumber stageNumber = StageNumber.Stage0;
    //選択可能なステージ
    public static StageNumber clearNumber = StageNumber.Stage1;

    //砂漠ステージ
    //ステージ名を順番に記入
    public enum SandStageNumber
    {
        Stage2,
        Stage2_2,
        Stage2_3,
    }
    [HideInInspector]
    public SandStageNumber sandNumber = 0;

    //ゲーム中の行動-------------------------------------
    public enum GameMode
    {
        play,
        pause,
        Help
    }
    [HideInInspector]
    public GameMode gameMode = GameMode.play;

    //ポーズ中の行動--------------------------------------
    enum PouseMode
    {
        Title,
        Help,
        Game
    }
    PouseMode pouseMode;

    Vector2 pouseSize = new Vector2(700f, 800f);
    Vector2 helpSize = new Vector2(1920f, 1080f);

    #region 取得用パス
    const string IMAGE_PATH = "Image/";
    const string POUSE_PATH = IMAGE_PATH + "Pause/";
    const string SKYBOX_PATH = IMAGE_PATH + "StageBack/";
    #endregion

    //Pauseの画像
    Texture[] images;
    RawImage pauseUi;                                   //ポーズのUI

    public const float BlackOutTime = 1.5f;               //ブラックアウトにかかる時間
    [HideInInspector]
    Image blackBack;                                      //ブラックアウトの色(画像)

    [HideInInspector]
    public bool coroutineFlag = false;                    //コルーチンが起動しているかどうか


    GameObject canvas;  //キャンバス
    GameObject stage;   //ステージ
    XBox xBox;

    GameObject player;
    PlayerMove playerScript;
    Cable cable;

    //ステージ
    [HideInInspector]
    public GameObject instansStage;
    [HideInInspector]
    public Vector3 startPosition;

    IEnumerator coroutine;  //コルーチンを入れる用のコルーチン

    AudioController audioC;

    Builder bilder;
    void Awake()
    {
        audioC = GetComponent<AudioController>();
        bilder = GetComponent<Builder>();
        //ステージの取得
        stage = Resources.Load<GameObject>("StagePrefab/" + stageNumber.ToString());
        //プレイヤーの取得
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerMove>();
        foreach (Transform child in player.transform)
        {
            if (child.gameObject.tag == "Cable")
            {
                cable = child.gameObject.GetComponent<Cable>();
                break;
            }
        }
        //ステージの生成
        instansStage = Instantiate(stage);

        //スカイボックスをステージに合わせる
        RenderSettings.skybox = Resources.Load<Material>(SKYBOX_PATH + stageNumber.ToString());

        foreach (Transform child in instansStage.transform)
        {
            if (child.gameObject.tag == "GimmickParent")
            {
                cable.gimmickParent = child.gameObject;
            }
        }

        //ビルダーにステージ情報を送る
        bilder.stage = instansStage;
        xBox = GetComponent<XBox>();

        //プレイヤーの初期位置の設定
        foreach (Transform child in instansStage.transform)
        {
            if (child.gameObject.tag == "StartPosition")
            {
                startPosition = child.transform.position;
                player.transform.position = startPosition;
                player.transform.eulerAngles = child.transform.eulerAngles;
            }
        }

        //キャンバスの取得
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        foreach (Transform child in canvas.transform)
        {
            if (child.gameObject.name == "Pause")
            {
                //ポーズ画面の取得
                pauseUi = child.gameObject.GetComponent<RawImage>();
            }
            //ブラックアウト用の画像の取得
            if (child.gameObject.name == "BlackOut")
            {
                blackBack = child.gameObject.GetComponent<Image>();
            }
        }

        //ポーズ画面の取得 +1はHelp画面を入れる
        images = new Texture[Enum.GetValues(typeof(PouseMode)).Length + 1];
        for (int i = 0; i < Enum.GetValues(typeof(PouseMode)).Length; i++)
        {
            PouseMode pm = (PouseMode)i;
            images[i] = Resources.Load<Texture>(POUSE_PATH + pm.ToString());
        }
        images[images.Length - 1] = Resources.Load<Texture>("Image/Help");

        //黒から透明に
        BlackOutAlpha(false, 1f, true, Color.black, BlackOutTime);

        //ステージにあったBGMの再生
        AudioController.Bgm bgm = (AudioController.Bgm)Enum.Parse(typeof(AudioController.Bgm), stageNumber.ToString() + "_Bgm");
        audioC.BgmSoundPlay(bgm);
    }

    //流砂でのステージ移動
    public void ChangeStage()
    {
        sandNumber++;
        //ステージの取得
        stage = Resources.Load<GameObject>("StagePrefab/" + sandNumber.ToString());
        Destroy(instansStage);
        //ステージの生成
        instansStage = Instantiate(stage);


        //ビルダーにステージ情報を送る
        bilder.stage = instansStage;

        foreach (Transform child in instansStage.transform)
        {
            if (child.gameObject.tag == "GimmickParent")
            {
                cable.gimmickParent = child.gameObject;
            }
            if (child.gameObject.name == "Enemy")
            {
                GameObject enemyParent;
                enemyParent = child.gameObject;
                enemyParent.SetActive(false);
                bilder.Default(enemyParent);
            }
        }

        //プレイヤーの初期位置の設定
        foreach (Transform child in instansStage.transform)
        {
            if (child.gameObject.tag == "StartPosition")
            {
                startPosition = child.transform.position;
                player.transform.position = startPosition;
                player.transform.eulerAngles = child.transform.eulerAngles;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMode != GameMode.play &&
            xBox.ButtonDown(XBox.Str.Start))
        {
            Time.timeScale = 1f;
            PlayGame();
        }
        //プレイ中にできること
        else if (gameMode == GameMode.play && !playerScript.stopPlayer)
        {
            //ポーズ画面の表示
            if ((xBox.ButtonDown(XBox.Str.Start)))
            {
                playerScript.stopPlayer = true;
                pauseUi.gameObject.SetActive(true);
                ChangePouseMode();
            }
        }
        //ポース中にできること
        else if (gameMode == GameMode.pause)
        {
            PauseMode();
        }
        else if (gameMode == GameMode.Help)
        {
            Time.timeScale = 0f;
            if ((xBox.ButtonDown(XBox.Str.A)))
            {
                ChangePouseMode();
            }
        }
    }

    //ポーズモードへ
    void ChangePouseMode()
    {
        SeSoundLoad(AudioController.Se.Enter);
        gameMode = GameMode.pause;  //矢印の場所変更
        pouseMode = 0;
        ImageChange((int)pouseMode);
    }

    void PauseMode()
    {
        Time.timeScale = 0f;
        //選択
        if (xBox.ButtonDown(XBox.Str.A))
        {
            SeSoundLoad(AudioController.Se.Enter);
            if (pouseMode == PouseMode.Help)
            {
                gameMode = GameMode.Help;
                ImageHelp();
            }
            else
            {
                Time.timeScale = 1f;
                if (pouseMode == PouseMode.Game)
                {
                    PlayGame();
                }
                else if (pouseMode == PouseMode.Title)
                {
                    LoadScene.sceneName = "Title";
                    SceneManager.LoadScene("Load");
                }
            }
        }

        //選択場所の変更
        if (xBox.ButtonDown(XBox.AxisStr.LeftButtonUp, true) ||
                xBox.ButtonDown(XBox.AxisStr.LeftButtonUp, false) ||
                xBox.ButtonDown(XBox.AxisStr.LeftJoyUp, true) ||
                xBox.ButtonDown(XBox.AxisStr.LeftJoyUp, false))
        {
            SeSoundLoad(AudioController.Se.Select);
            if (xBox.ButtonDown(XBox.AxisStr.LeftButtonUp, false) ||
                xBox.ButtonDown(XBox.AxisStr.LeftJoyUp, true))
            {
                if ((int)pouseMode + 1 < Enum.GetValues(typeof(PouseMode)).Length)
                {
                    pouseMode++;
                }
                else
                {
                    pouseMode = 0;
                }
            }
            else
            {
                if ((int)pouseMode > 0)
                {
                    pouseMode--;
                }
                else
                {
                    pouseMode = (PouseMode)(Enum.GetValues(typeof(PouseMode)).Length - 1);
                }
            }
            ImageChange((int)pouseMode);
        }
    }

    void ImageHelp()
    {
        ImageChange(images.Length - 1);
        pauseUi.rectTransform.sizeDelta = helpSize;
    }

    void ImageChange(int i)
    {
        if(i != images.Length - 1)
        {
            pauseUi.rectTransform.sizeDelta = pouseSize;
        }

        pauseUi.texture = images[i];
    }

    //ゲームに戻る処理
    void PlayGame()
    {
        playerScript.stopPlayer = false;
        pauseUi.gameObject.SetActive(false);
        gameMode = GameMode.play;
    }

    //Scene移動時
    public void SceneLoad(string str)
    {
        LoadScene.sceneName = str;
        SceneManager.LoadScene("Load");
    }

    //2重にコルーチンが処理されないように
    public void BlackOutAlpha(bool flag, float startAlpha, bool timeFlag, Vector4 color, float time)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = BlackOut(flag, startAlpha, timeFlag, color, time);
        StartCoroutine(coroutine);
    }

    //flag true = 透明→黒 false = 黒→透明 startAlpha初期のアルファ値 timeFlag 時間を止めるかどうか color色 time 暗くなったり明るくなったりする時間
    public IEnumerator BlackOut(bool flag, float startAlpha, bool timeFlag, Vector4 color, float timer)
    {
        coroutineFlag = true;
        float time;
        //時間の決定
        if (flag)
        {
            time = (1f - startAlpha) * timer;
        }
        else
        {
            time = startAlpha * timer;
        }

        //時を止めるかどうか
        if (timeFlag)
        {
            playerScript.stopPlayer = true;
            Time.timeScale = 0f;
        }
        //色の変更
        blackBack.color = color;
        //アルファをフラグがtrueなら暗くfalseなら明るくしていく

        float alpha = startAlpha;
        //透明度の変更
        do
        {
            alpha = Mathf.MoveTowards(alpha, flag ? 1f : 0f, Time.unscaledDeltaTime / time);
            blackBack.color = new Color(blackBack.color.r, blackBack.color.g, blackBack.color.b, alpha);
            yield return null;
        }
        while (alpha != 0f && alpha != 1f);

        //時間を止めていたら時を戻す
        if (timeFlag)
        {
            Time.timeScale = 1f;
        }

        //黒ならステージリトライ。透明ならゲーム再開
        if (flag)
        {
            LoadScene.sceneName = "Stage";
            SceneManager.LoadScene("Load");
        }
        else
        {
            playerScript.stopPlayer = false;
        }
        coroutineFlag = false;
        yield break;
    }

    //音源関係

    public void SeSoundLoad(AudioController.Se audioClip)
    {
        audioC.SeSoundPlay(audioClip);
    }

    public void SeSoundLoad(AudioController.Se audioClip,out GameObject go)
    {
        audioC.SeSoundPlay(audioClip,out go);
    }

    public void SeSoundLoad(AudioController.Se audioClip,float volme)
    {
        audioC.SeSoundPlay(audioClip, volme);
    }

    public void SeSoundLoad(AudioController.Se audioClip, out GameObject go,float volme)
    {
        audioC.SeSoundPlay(audioClip, out go, volme);
    }

    public void BgmSoundLoad(AudioController.Bgm audioClip)
    {
        audioC.BgmSoundPlay(audioClip);
    }

    public void BgmSoundLoad(AudioController.Bgm audioClip,out AudioSource audio)
    {
        audioC.GimmckSoundBgmPlay(audioClip,out audio);
    }
}
