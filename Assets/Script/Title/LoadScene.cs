using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    //ロード関係---------------------------------------
    public static string sceneName; 
    AsyncOperation loadScene;      //ロード先
    const float LoadTimeMin = 2f;  //最低のロード時間
    //------------------------------------------------
    void Start ()
    {
        Time.timeScale = 1f;
        loadScene = SceneManager.LoadSceneAsync(sceneName);
        loadScene.allowSceneActivation = false;
        StartCoroutine(load());
    }

    IEnumerator load()
    {
        yield return new WaitForSeconds(LoadTimeMin);
        loadScene.allowSceneActivation = true;
        yield break;
    }
}
