using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : SwitchFlag
{
    Vector3 CURVE_HEIGHT = Vector3.up * 0.2f;

    public enum MeshMode
    {
        Wide,       //横長
        Height,     //縦長
        Curve       //角
    }

    [HideInInspector] public Vector3 up;
    [HideInInspector] public Vector3 down;
    [HideInInspector] public Vector3 right;
    [HideInInspector] public Vector3 forward;

    public Mesh MeshCreate(List<Mesh> meshes)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertex = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        for (int i = 0;i < meshes.Count;i++)
        {
            int[] count = meshes[i].triangles;

            for (int j = 0;j < meshes[i].triangles.Length;j++)
            {
                count[j] += vertex.Count;
                triangles.Add(count[j]);
            }

            for(int j = 0;j < meshes[i].vertices.Length;j++)
            {
                vertex.Add(meshes[i].vertices[j]); 
                uv.Add(meshes[i].uv[j]);
            }
        }

        mesh.vertices = vertex.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        
        return mesh;
    }

    public Mesh MeshCreate(Vector3 start, Vector3 end, MeshMode mode,bool flag)
    {
        Mesh createMesh = new Mesh();
        start -= transform.position;
        end -= transform.position;

        if (mode == MeshMode.Wide)
        {
            createMesh.vertices = VertexPosWide(start, end);
            createMesh.triangles = Triangle(createMesh.vertices);
            createMesh.uv = UvMapWideCurve(createMesh.vertices);
        }
        else if (mode == MeshMode.Height)
        {
            createMesh.vertices = VertexPosHight(start, end);
            createMesh.triangles = Triangle(createMesh.vertices);
            createMesh.uv = UvMapHight(createMesh.vertices);
        }
        else
        {
            if(flag)
            {
                createMesh.vertices = VertexPosCurve2(start, end);
            }
            else
            {
                createMesh.vertices = VertexPosCurve(start, end);
            }
            createMesh.triangles = Triangle(createMesh.vertices);
            createMesh.uv = UvMapWideCurve(createMesh.vertices);
        }

        createMesh.RecalculateNormals();
        return createMesh;
    }

    void GetVector3(ref Vector3 vec3,ref Vector3 vec32)
    {
        vec3 =  new Vector3(Get3(vec3).x,vec3.y, Get3(vec3).z);
        vec32 =  new Vector3(Get3(vec32).x, vec32.y, Get3(vec32).z);
    }

    Vector3 Get3(Vector3 vec3)
    {
        return new Vector3(vec3.x, 0f, vec3.z).magnitude * Vector3.forward;
    }

    //地面の頂点の設定
    Vector3[] VertexPosWide(Vector3 start, Vector3 end)
    {
        GetVector3(ref start, ref end);
        List<Vector3> vec3 = new List<Vector3>();

        //上面
        vec3.Add(start + Vector3.right + up);
        vec3.Add(start + up);
        vec3.Add(end + Vector3.right + up);
        vec3.Add(end + up);

        //左面
        vec3.Add(start + up);
        vec3.Add(start);
        vec3.Add(end + up);
        vec3.Add(end);

        //右面
        vec3.Add(start + Vector3.right);
        vec3.Add(start + Vector3.right + up);
        vec3.Add(end + Vector3.right);
        vec3.Add(end + Vector3.right + up);

        //奥
        vec3.Add(end + up + Vector3.right);
        vec3.Add(end + up);
        vec3.Add(end + Vector3.right);
        vec3.Add(end);

        //手前
        vec3.Add(start + up);
        vec3.Add(start + up + Vector3.right);
        vec3.Add(start);
        vec3.Add(start + Vector3.right);

        return vec3.ToArray();
    }

    //空中の頂点の設定
    Vector3[] VertexPosHight(Vector3 start, Vector3 end)
    {
        GetVector3(ref start, ref end);
        List<Vector3> vec3 = new List<Vector3>();

        //奥
        vec3.Add(start + Vector3.right);
        vec3.Add(start);
        vec3.Add(end + Vector3.right);
        vec3.Add(end);

        //左面
        vec3.Add(start);
        vec3.Add(start - Vector3.forward);
        vec3.Add(end);
        vec3.Add(end - Vector3.forward);

        //右面
        vec3.Add(start + Vector3.right - Vector3.forward);
        vec3.Add(start + Vector3.right);
        vec3.Add(end + Vector3.right - Vector3.forward);
        vec3.Add(end + Vector3.right);

        //手前
        vec3.Add(start - Vector3.forward);
        vec3.Add(start + Vector3.right - Vector3.forward);
        vec3.Add(end - Vector3.forward);
        vec3.Add(end + Vector3.right - Vector3.forward);

        //下
        vec3.Add(end - Vector3.forward);
        vec3.Add(end - Vector3.forward + Vector3.right);
        vec3.Add(end);
        vec3.Add(end + Vector3.right);

        //上
        vec3.Add(start - Vector3.forward + Vector3.right);
        vec3.Add(start - Vector3.forward);
        vec3.Add(start + Vector3.right);
        vec3.Add(start);

        return vec3.ToArray();
    }
    
    //カーブの頂点の設定
    Vector3[] VertexPosCurve(Vector3 start, Vector3 end)
    {
        GetVector3(ref start, ref end);
        List<Vector3> vec3 = new List<Vector3>();
        //上面
        vec3.Add(start + Vector3.right + up);
        vec3.Add(start + up);
        vec3.Add(end + Vector3.right + CURVE_HEIGHT);
        vec3.Add(end + CURVE_HEIGHT);

        //左面
        vec3.Add(start + up);
        vec3.Add(start);
        vec3.Add(end + CURVE_HEIGHT);
        vec3.Add(end);

        //右面
        vec3.Add(start + Vector3.right);
        vec3.Add(start + Vector3.right + up);
        vec3.Add(end + Vector3.right);
        vec3.Add(end + Vector3.right + CURVE_HEIGHT);

        //奥
        vec3.Add(end + CURVE_HEIGHT + Vector3.right);
        vec3.Add(end + CURVE_HEIGHT);
        vec3.Add(end + Vector3.right);
        vec3.Add(end);

        //手前
        vec3.Add(start + up);
        vec3.Add(start + up + Vector3.right);
        vec3.Add(start);
        vec3.Add(start + Vector3.right);

        return vec3.ToArray();
    }

    //カーブの頂点の設定
    Vector3[] VertexPosCurve2(Vector3 start, Vector3 end)
    {
        GetVector3(ref start, ref end);
        List<Vector3> vec3 = new List<Vector3>();
        float range = (start - end).magnitude;
        //上面
        vec3.Add(start + Vector3.right + up * range);
        vec3.Add(start + up * range);
        vec3.Add(end + Vector3.right + CURVE_HEIGHT * range);
        vec3.Add(end + CURVE_HEIGHT * range);

        //左面
        vec3.Add(start + up * range);
        vec3.Add(start);
        vec3.Add(end + CURVE_HEIGHT * range);
        vec3.Add(end);

        //右面
        vec3.Add(start + Vector3.right);
        vec3.Add(start + Vector3.right + up * range);
        vec3.Add(end + Vector3.right);
        vec3.Add(end + Vector3.right + CURVE_HEIGHT * range);

        //奥
        vec3.Add(end + CURVE_HEIGHT * range + Vector3.right);
        vec3.Add(end + CURVE_HEIGHT * range);
        vec3.Add(end + Vector3.right);
        vec3.Add(end);

        //手前
        vec3.Add(start + up * range);
        vec3.Add(start + up * range + Vector3.right);
        vec3.Add(start);
        vec3.Add(start + Vector3.right);

        return vec3.ToArray();
    }
    //横とカーブ時-------------------------------------------

    //つなげる設定
    int[] Triangle(Vector3[] vec3)
    {
        List<int> Pos = new List<int>();
        //if(MoveAngle.z != 0f)
        {
            for (int i = 0; i < vec3.Length / 4; i++)
            {
                int add = i * 4;

                Pos.Add(add);
                Pos.Add(add + 1);
                Pos.Add(add + 2);

                Pos.Add(add + 2);
                Pos.Add(add + 1);
                Pos.Add(add + 3);
            }
        }
          
        return Pos.ToArray();
    }

    //地面とカーブ時-----------------------------------------

    //UVMAPの生成
    Vector2[] UvMapWideCurve(Vector3[] vertexPos)
    {
        List<Vector2> vec2 = new List<Vector2>();
        int i = 0;

        UvUp(vertexPos, ref vec2, ref i);
        UvGroundSide(vertexPos, ref vec2, ref i);
        UvGroundSide(vertexPos, ref vec2, ref i);
        UvDown(vertexPos, ref vec2, ref i);
        UvDown(vertexPos, ref vec2, ref i);


        return vec2.ToArray();
    }

    //空中時------------------------------------------------

    //UVMAPの生成
    Vector2[] UvMapHight(Vector3[] vertexPos)
    {
        List<Vector2> vec2 = new List<Vector2>();
        int i = 0;

        UvDown(vertexPos, ref vec2, ref i);
        UvSkySide(vertexPos, ref vec2, ref i);
        UvSkySide(vertexPos, ref vec2, ref i);
        UvDown(vertexPos, ref vec2, ref i);
        UvUp(vertexPos, ref vec2, ref i);
        UvUp(vertexPos, ref vec2, ref i);

        return vec2.ToArray();
    }

    //上面
    void UvUp(Vector3[] vec3, ref List<Vector2> vec2, ref int i)
    {
        do
        {
            vec2.Add(new Vector2(vec3[i].x, vec3[i].z));
            i++;
        }
        while (i % 4 != 0);
    }

    //側面
    void UvGroundSide(Vector3[] vec3, ref List<Vector2> vec2, ref int i)
    {
        do
        {
            vec2.Add(new Vector2(vec3[i].y, vec3[i].z));
            i++;
        }
        while (i % 4 != 0);
    }

    //側面
    void UvSkySide(Vector3[] vec3, ref List<Vector2> vec2, ref int i)
    {
        do
        {
            vec2.Add(new Vector2(-vec3[i].z, -vec3[i].y));
            i++;
        }
        while (i % 4 != 0);
    }

    //奥面
    void UvDown(Vector3[] vec3, ref List<Vector2> vec2, ref int i)
    {
        do
        {
            vec2.Add(new Vector2(-vec3[i].x, -vec3[i].y));
            i++;
        }
        while (i % 4 != 0);
    }
}