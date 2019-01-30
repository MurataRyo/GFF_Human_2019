using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenbanMovie : MonoBehaviour {

    //連番画像の格納先
    [SerializeField]
    string imagePath;
    //格納先
    Texture[] image;
    //描画先のRawImage
    [SerializeField]
    public RawImage titleUi;
    //画像切り替えの速度
    [SerializeField]
    float speed;

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
                titleUi.texture = image[i];
                yield return new WaitForSeconds(speed);
            }
        }
    }
}
