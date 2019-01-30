using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Magma02 : CreateMesh
{
    MeshMode nowMode = MeshMode.Wide;
    MeshMode nowModeLog = MeshMode.Wide;

    const float SPEED = 4f;

    Material material;

    LayerMask blockMask;          //レイで取得するレイヤー 

    MeshFilter meshFilter;

    List<Meshes> meshes = new List<Meshes>();

    bool moveFlag = true;
    bool flagLog;

    [SerializeField]
    float startSpeed;

    [SerializeField]
    float endSpeed;

    public void Start()
    {
        up = Vector3.up;
        down = -Vector3.up;
        right = transform.right;
        forward = transform.forward;

        blockMask = LayerMask.GetMask(new string[] { "Ground", "Block" });

        material = Resources.Load<Material>("Material/Magma");

        gameObject.AddComponent<MeshRenderer>().material = material;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshAdd();
        flagLog = moveFlag;

        StartCoroutine(Cor());
    }

    void MeshAdd()
    {
        meshes.Add(gameObject.AddComponent<Meshes>());
        meshes[meshes.Count - 1].OneAdd(OneReturn(transform.localPosition));
        StartCoroutine(meshes[meshes.Count - 1].Move());
    }

    public void MeshDestory()
    {
        Meshes mesh = meshes[0];
        for(int i = 0;i < meshes[0].boxColliders.Count;i++)
        {
            meshes[0].boxColliders.Remove(meshes[0].boxColliders[i]);
        }
        meshes.Remove(mesh);
        Destroy(mesh);
    }

    //リストの追加
    public One OneReturn(Vector3 now)
    {
        One one = new One(now, NextPosUpdate(now), nowMode);
        return one;
    }

    void Update()
    {
        List<Mesh> mesh = new List<Mesh>();
        for (int i = 0; i < meshes.Count; i++)
        {
            for (int j = 0; j < meshes[i].ones.Count; j++)
            {
                bool b = j == 0 && !meshes[i].move;
                mesh.Add(MeshCreate(meshes[i].ones[j].start, meshes[i].ones[j].now, meshes[i].ones[j].meshMode, b));
            }
        }

        meshFilter.mesh = MeshCreate(mesh);

        if (!flagLog && moveFlag)
        {
            MeshAdd();
        }
        flagLog = moveFlag;
    }

    //次の座標の更新
    Vector3 NextPosUpdate(Vector3 pos)
    {
        nowModeLog = nowMode;

        nowMode = NextMode(pos, nowMode);

        int i = 0;
        Vector3 move;
        if (nowMode == MeshMode.Wide)
        {
            move = forward;
        }
        else
        {
            move = down;
        }

        MeshMode log = nowMode;
        do
        {
            if (nowMode == MeshMode.Height && (pos + (down * (i - 1))).y < -6f)
            {
                break;
            }
            nowMode = NextMode(pos + (move * i), nowMode);
            i++;
        }
        while (nowMode == nowModeLog);

        nowMode = log;

        if (nowMode == MeshMode.Wide || nowMode == MeshMode.Curve)
        {
            return pos + (forward * (i - 1));
        }
        else
        {
            return pos + (down * (i - 1));
        }

    }

    //次のモードを調べる
    MeshMode NextMode(Vector3 pos, MeshMode nowMode)
    {
        if (nowMode == MeshMode.Wide)
        {
            if (Physics.Linecast(pos + (forward * 0.95f) + up, pos + (forward * 0.95f), blockMask))
            {
                return MeshMode.Wide;
            }
            else
            {
                return MeshMode.Curve;
            }
        }
        else
        {
            if (Physics.Linecast(pos + up - (forward * 0.05f), pos - (forward * 0.05f), blockMask))
            {
                return MeshMode.Wide;
            }
            else
            {
                return MeshMode.Height;
            }
        }
    }

    //小数点以下切り捨て
    void DownIf(ref float i)
    {
        if (i % 1 >= 0.001f)
        {
            i -= i % 1;
        }
    }
    void DownIf(ref Vector3 i)
    {
        DownIf(ref i.x);
        DownIf(ref i.y);
        DownIf(ref i.z);
    }

    //つながっているマグマの情報
    public class Meshes : MonoBehaviour
    {
        public bool endFlag = false;
        public bool move = true;
        public List<One> ones = new List<One>();
        Magma02 magma;
        public List<BoxCollider> boxColliders = new List<BoxCollider>();

        public void OneAdd(One one)
        {
            ones.Add(one);
            BoxAdd();
        }

        void BoxAdd()
        {
            boxColliders.Add(gameObject.AddComponent<BoxCollider>());
            boxColliders[boxColliders.Count - 1].isTrigger = true;
        }

        //移動
        public IEnumerator Move()
        {
            magma = GetComponent<Magma02>();
            while (true)
            {
                if(!endFlag)
                {
                    ones[ones.Count - 1].now = Vector3.MoveTowards(ones[ones.Count - 1].now, ones[ones.Count - 1].end, SPEED * Time.deltaTime);

                    if (ones[ones.Count - 1].now == ones[ones.Count - 1].end)
                    {
                        if (ones[ones.Count - 1].now.y > -6f)
                        {
                            ones.Add(magma.OneReturn(ones[ones.Count - 1].now));
                            BoxAdd();
                        }
                    }
                    if (move && !magma.moveFlag)
                    {
                        move = false;
                    }
                }
                
                if (!move)
                {
                    ones[0].start = Vector3.MoveTowards(ones[0].start, ones[0].end, SPEED * Time.deltaTime);
                    if (ones[0].start == ones[0].end)
                    {
                        if (ones.Count == 1)
                        {
                            Destroy(boxColliders[0]);
                            boxColliders.Remove(boxColliders[0]);
                            magma.MeshDestory();
                            yield break;
                        }
                        else
                        {
                            ones.Remove(ones[0]);
                            Destroy(boxColliders[0]);
                            boxColliders.Remove(boxColliders[0]);
                        }
                    }
                }

                for (int i = 0; i < ones.Count; i++)
                {
                    ColCreate(i);
                }

                yield return null;
            }
        }

        void ColCreate(int i)
        {
            if (!move && i == 0)
            {
                boxColliders[i].size = Size(ones[i].start, ones[i].now, ones[i].meshMode);
                boxColliders[i].center = Center(ones[i].start, ones[i].now, ones[i].meshMode);
            }
            else
            {
                boxColliders[i].size = Size(ones[i].start, ones[i].now, ones[i].meshMode);
                boxColliders[i].center = Center(ones[i].start, ones[i].now, ones[i].meshMode);
            }
        }

        Vector3 Size(Vector3 start, Vector3 end, MeshMode meshMode)
        {
            if (meshMode == MeshMode.Height)
            {
                return new Vector3(1f, (start - end).magnitude, 1f);
            }
            else
            {
                return new Vector3(1f, 1f, (start - end).magnitude);
            }
        }

        Vector3 Center(Vector3 start, Vector3 end, MeshMode meshMode)
        {
            float rangeZ = new Vector3(start.x - transform.position.x, 0f, start.z - transform.position.z).magnitude;
            float rangeY = start.y - transform.position.y;
            if (meshMode == MeshMode.Height)
            {
                return new Vector3(0.5f, -(start - end).magnitude / 2 + rangeY, -0.5f + rangeZ);
            }
            else
            {
                return new Vector3(0.5f, 0.5f + rangeY, (start - end).magnitude / 2 + rangeZ);
            }
        }
    }

    public class One
    {
        public MeshMode meshMode;
        public Vector3 start;
        public Vector3 end;
        public Vector3 now;

        public One(Vector3 s, Vector3 e, MeshMode m)
        {
            meshMode = m;
            start = now = s;
            end = e;
        }
    }

    IEnumerator Cor()
    {
        while(true)
        {
            if(flag)
            {
                yield return new WaitForSeconds(moveFlag ? endSpeed : startSpeed);
                moveFlag = !moveFlag;
            }
            else
            {
                moveFlag = false;
                yield return null;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.GetComponent<PlayerMove>().Damage();
        }
    }
}
