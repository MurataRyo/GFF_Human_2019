//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class NewBehaviourScript : MonoBehaviour {
//    [SerializeField] LayerMask mask;
//	// Use this for initialization
//	void Start () {
//        AddMesh(gameObject);

//    }

//    void AddMesh(GameObject go)
//    {
//        foreach(Transform child in go.transform)
//        {
//            child.gameObject.layer = mask;
//            AddMesh(child.gameObject);
//        }
//    }
	
//	// Update is called once per frame
//	void Update () {
		
//	}
//}


//class Base
//{
//    int x;
//    int y;
//    int z;

//    Base(int x,int y,int z)
//    {
//        this.x = x;
//        this.y = y;
//        this.z = z;
//    }

//    public static Base operator+ (Base w,Base h)
//    {
//        return new Base(w.x + h.x,w.y + h.y,w.z+h.z);
//    }

//    public static Base operator- (Base w, Base h)
//    {
//        return new Base(w.x - h.x, w.y - h.y, w.z - h.z);
//    }
//}

//class EnumMode
//{
//    enum Text
//    {
//        AMode,
//        BMode,
//        CMode
//    }


//}

//public class Mode
//{
//    int i;
//    int j;
//    int k;
//    public virtual Mode(int I,int J)
//    {
//        i = I;
//        j = J;
//        k = I + J;
//    }
//}

//public class AMode : Mode
//{
//    override Mode(int I,int J)
//    {
//        base(I,J);
//        k = I * J;
//    }
//}

//public class BMode : CMode , AMode
//{

//}

//public class CMode
//{
//    int i;
//}

