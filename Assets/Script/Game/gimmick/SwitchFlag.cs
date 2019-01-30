using UnityEngine;

public class SwitchFlag : MonoBehaviour
{
    public bool flag;
    void Update()
    {
        if(GetComponent<Animator>())
        {
            Animator ani = GetComponent<Animator>();
            ani.SetBool(gameObject.name + "Flag", flag);
        }
    }
}