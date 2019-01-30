using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaController : MonoBehaviour {
    public List<Vector3> vector3 = new List<Vector3>();

    public bool CreateOk(Vector3 pos,Vector3 forward)
    {
        Vector3 right = new Vector3(forward.z, 0f, -forward.x);
        pos += (right + forward) * 0.5f;
        for (int i = 0;i < vector3.Count; i++)
        {
            if(vector3[i] == pos)
            {
                return false;
            }
        }
        return true;
    }

    public void AddPos(Vector3 pos, Vector3 forward)
    {
        Vector3 right = new Vector3(forward.z, 0f, forward.x);
        pos += (right + forward) * 0.5f;
        vector3.Add(pos);
    }
}
