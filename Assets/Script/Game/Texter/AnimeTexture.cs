using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeTexture : MonoBehaviour
{
    [SerializeField]
    float speed;
    Vector2 randRad = new Vector2(0f, 1f);
    Vector2 uvPos = Vector2.zero;
    Mesh mesh;

    IEnumerator Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        while (true)
        {
            uvPos += randRad * speed * Time.deltaTime;
            OverUv(ref uvPos);
            mesh.uv = UvMapCreates();
            yield return null;
        }
    }

    void OverUv(ref Vector2 vec2)
    {
        if (vec2.x > 1f)
        {
            vec2.x -= 1f;
        }
        if (vec2.y > 1f)
        {
            vec2.y -= 1f;
        }
    }

    Vector2[] UvMapCreates()
    {
        List<Vector2> Pos = new List<Vector2>();
        Pos.Add(uvPos);
        Pos.Add(uvPos + new Vector2(1f, 1f));
        Pos.Add(uvPos + new Vector2(1f, 0f));
        Pos.Add(uvPos + new Vector2(0f, 1f));
        return Pos.ToArray();
    }
}
