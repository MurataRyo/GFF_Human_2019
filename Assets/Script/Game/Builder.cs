using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    [SerializeField] Vector3 size;      // NavMeshを生成する範囲
    List<MeshFilter> meshes;

    //読み込む場所
    [HideInInspector]
    public GameObject stage;

    NavMeshData data;
    void Start()
    {
        Default();
        StartCoroutine(StartCor());
    }

    public void Default()
    {
        NavMesh.RemoveAllNavMeshData();
        meshes = new List<MeshFilter>();
        LoadMesh(stage);
        data = new NavMeshData();
        NavMesh.AddNavMeshData(data);
    }

    public void Default(GameObject enemy)
    {
        NavMesh.RemoveAllNavMeshData();
        meshes = new List<MeshFilter>();
        LoadMesh(stage);
        data = new NavMeshData();
        NavMesh.AddNavMeshData(data);
        if (enemy != null)
        {
            enemy.SetActive(true);
        }
    }


    //ステージを子要素をすべて探す
    void LoadMesh(GameObject go)
    {
        //子要素の中からタグ付きのだけをリストに入れる
        foreach (Transform child in go.transform)
        {
            if (child.gameObject.tag == "Mesh" ||
                child.gameObject.tag == "GimmickCol")
            {
                if(child.gameObject.GetComponent<MeshFilter>())
                {
                    meshes.Add(child.gameObject.GetComponent<MeshFilter>());
                }
            }
            LoadMesh(child.gameObject);
        }
    }

    IEnumerator StartCor()
    {
        while (true)
        {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
            Bounds bounds = new Bounds(transform.position + size / 2, size);

            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            foreach (var mesh in meshes)
            {
                NavMeshBuildSource source = new NavMeshBuildSource();
                source.shape = NavMeshBuildSourceShape.Mesh;
                source.sourceObject = mesh.sharedMesh;
                source.transform = mesh.transform.localToWorldMatrix;
                source.area = 0;

                sources.Add(source);
            }

            NavMeshBuilder.UpdateNavMeshData(data, settings, sources, bounds);
            yield return null;
        }
    }
}
