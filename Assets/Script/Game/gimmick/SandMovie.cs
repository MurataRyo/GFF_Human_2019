using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandMovie : MonoBehaviour
{

    //連番画像の格納先
    string imagePath = "Image/Quicksand";
    //格納先
    Texture[] image;
    //描画先のマテリアル
    [SerializeField]
    Material material;
    //画像切り替えの速度
    float speed = 0.1f;

    void Start()
    {
        //格納
        image = Resources.LoadAll<Texture>(imagePath);
        StartCoroutine(Cor());
    }

    //連番画像の反映
    IEnumerator Cor()
    {
        while (true)
        {
            //描画
            for (int i = 0; i < image.Length; i++)
            {
                material.mainTexture = image[i];
                yield return new WaitForSeconds(speed / image.Length);
            }
        }
    }
}