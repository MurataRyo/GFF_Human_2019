using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {
    
    [HideInInspector]   public AudioClip clip;

    const float BGM_VOLUME = 0.5f;

    const string BGM_PATH = "Audio/Bgm";
    [HideInInspector]public const string SE_PATH = "Audio/Se";

    #region 音源へのパス

    //BGMとSEは「フォルダ名_ファイル名」とかく

    public enum Bgm
    {
        Title_Bgm,
        Stage0_Bgm,
        Stage1_Bgm,
        Stage2_Bgm,
        Stage3_Bgm,
        Gimmick_Beltconveyor,
        Gimmick_Wind,
    }

    public enum Se
    {
        Player_Damage,
        Player_UsbSet,
        Player_UsbPower,
        Player_Gun,
        Enter,
        Select,
        Gimmick_Bridge,
        Gimmick_HoleGround,
        Gimmick_HoleVacant,
        Gimmick_Water,
        Enemy_Destory,
    }

    #endregion

    #region 関数

    #region　BGM(ループ)
    //BGM音源
    public void BgmSoundPlay(Bgm str)
    {
        string path = BGM_PATH + File(str.ToString());
        clip = Resources.Load<AudioClip>(path);

        GameObject go = Instantiate(Resources.Load<GameObject>("GameObject/AudioChild"));
        go.transform.parent = transform;
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.loop = true;
        audio.clip = clip;
        audio.Play();
        audio.volume = BGM_VOLUME;
    }

    //ループギミックの音
    public void GimmckSoundBgmPlay(Bgm str,out AudioSource audio)
    {
        string path = BGM_PATH + File(str.ToString());
        clip = Resources.Load<AudioClip>(path);

        GameObject go = Instantiate(Resources.Load<GameObject>("GameObject/AudioChild"));
        go.transform.parent = transform;
        audio = go.GetComponent<AudioSource>();
        audio.loop = true;
        audio.clip = clip;
    }

    #endregion

    #region　SE(単発)

    //SE音源の再生
    public void SeSoundPlay(Se str, out GameObject go)
    {
        string path = SE_PATH + File(str.ToString());
        clip = Resources.Load<AudioClip>(path);

        go = Instantiate(Resources.Load<GameObject>("GameObject/AudioChild"));
        go.transform.parent = transform;
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.loop = false;
        audio.clip = clip;
        audio.Play();
        Destroy(go, clip.length);
    }

    //SE音源の再生
    public void SeSoundPlay(Se str)
    {
        string path = SE_PATH + File(str.ToString());
        clip = Resources.Load<AudioClip>(path);

        GameObject go = Instantiate(Resources.Load<GameObject>("GameObject/AudioChild"));
        go.transform.parent = transform;
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.loop = false;
        audio.clip = clip;
        audio.Play();
        Destroy(go, clip.length);
    }

    //SE音源の再生
    public void SeSoundPlay(Se str, out GameObject go,float volume)
    {
        string path = SE_PATH + File(str.ToString());
        clip = Resources.Load<AudioClip>(path);

        go = Instantiate(Resources.Load<GameObject>("GameObject/AudioChild"));
        go.transform.parent = transform;
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.loop = false;
        audio.clip = clip;
        audio.Play();
        audio.volume = volume;
        Destroy(go, clip.length);
    }

    //SE音源の再生
    public void SeSoundPlay(Se str, float volume)
    {
        string path = SE_PATH + File(str.ToString());
        clip = Resources.Load<AudioClip>(path);

        GameObject go = Instantiate(Resources.Load<GameObject>("GameObject/AudioChild"));
        go.transform.parent = transform;
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.loop = false;
        audio.clip = clip;
        audio.Play();
        audio.volume = volume;
        Destroy(go, clip.length);
    }

    #endregion

    //ファイルの取得
    public string File(string str)
    {
        string[] str2 = str.Split(char.Parse("_"));
        string outStr = null;
        for(int i = 0; i < str2.Length;i++)
        {
            outStr += "/" + str2[i];
        }
        return outStr;
    }
    #endregion
}
