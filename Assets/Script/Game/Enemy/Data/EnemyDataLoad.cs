using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EnemyData
{
    public Enemy[] data;
}

public class EnemyDataLoad : MonoBehaviour
{
    public static string
        [][] enemyData;     //敵のデータ
    public static EnemyData EnemyData2;
    // Use this for initialization
    void Start()
    {
        //テキストデータの読み込み+書き込み
        EnemyData2 = JsonUtility.FromJson<EnemyData>(OpenTextFile(Application.dataPath + "/enemyData.txt"));
    }


    //テキストから文字を取得
    public string OpenTextFile(string _filePath)
    {
        System.IO.FileInfo fi = new System.IO.FileInfo(_filePath);
        string returnSt = "";
        try
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fi.OpenRead(), System.Text.Encoding.Default))
            {
                returnSt = sr.ReadToEnd();
            }
        }

        catch (Exception error)
        {
            print(error.Message);
            returnSt = "READ_ERROR" + _filePath;
        }

        return returnSt;
    }
}
