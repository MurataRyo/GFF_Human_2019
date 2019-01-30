using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungBullet : MonoBehaviour
{
    Rigidbody rBody;
    float speed = 10f;
    float mass = 200;
    bool isGround = true;
    float gravity = 0f;
    float conveyorGravity = 0f;
    const float gravitySize = -9.81f;
    const float DESTORY_TIME = 2f;
    const float END_TIME = 10f;
    const float END_LIMIT_TIME = 4f;
    float limitTimer = 0f;
    bool endFlag = false;
    float timer = 0f;
    float radius = 0.5f;
    float height = 0.25f;
    
    Vector3 power;
    Vector3 velosity = Vector3.zero;
    LayerMask blockMask;

    // Use this for initialization
    void Start()
    {
        transform.parent = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().instansStage.transform;
        blockMask = (1 << LayerMask.NameToLayer("Block")) + (1 << LayerMask.NameToLayer("CableBlock")) 
                    + (1 << LayerMask.NameToLayer("Transparent")) + (1 << LayerMask.NameToLayer("Ground") + (1 << LayerMask.NameToLayer("MoveBlock")));
        rBody = gameObject.AddComponent<Rigidbody>();
        rBody.useGravity = false;
        rBody.freezeRotation = true;
        rBody.mass = mass;
        power = new Vector3((transform.forward * speed).x, 0f, (transform.forward * speed).z);
    }

    // Update is called once per frame
    void Update()
    {
        isGround = false;
        if(!endFlag)
        {
            GroundUpDate();
            PowerChange();
        }
        GravityControl();

        velosity = new Vector3(0f, gravity, 0f) + power;
        rBody.velocity = velosity;

        //ベルトコンベヤーの力を元に戻す
        power.y -= conveyorGravity;
        conveyorGravity = 0f;
        
        if (!endFlag && Timer(ref timer,END_TIME))
        {
            endFlag = true;
        }
        else if(endFlag && Timer(ref timer, DESTORY_TIME))
        {
            Destroy(gameObject);
        }

    }

    //重力の処理
    void GravityControl()
    {
        //地上
        if (isGround)
        {
            gravity = 0f;
        }
        //空中
        else
        {
            gravity += gravitySize * Time.deltaTime;
        }
    }

    void GroundUpDate()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + new Vector3(0f, height + radius, 0f), radius, -Vector3.up, out hit, height, blockMask))
        {
            if (!hit.collider.isTrigger &&
                hit.collider.gameObject.GetComponent<VelosityBlock>())
            {
                Vector3 vec = Vector3.zero;
                vec = hit.collider.gameObject.GetComponent<VelosityBlock>().VelosityObject(ref gravity);
                //その力を返す
                power += vec * Time.deltaTime;
                conveyorGravity = vec.y;
            }
            isGround = true;
        }
    }

    void PowerChange()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0f,height, 0f) + (power.normalized),-Vector3.up,out hit,height,blockMask))
        {
            float speedPower = power.magnitude;
            //傾き（未完成）
            //Vector3 addPower = -((hit.point - transform.position).normalized * (hit.point - transform.position).normalized.y) * (4f * Time.deltaTime);
            //パワーを坂に合わせる
            power = (hit.point - transform.position).normalized * speedPower;
        }
        else if(Timer(ref limitTimer,END_LIMIT_TIME))
        {
            endFlag = true;
            timer = 0f;
        }
    }

    bool Timer(ref float timer,float time)
    {
        timer += Time.deltaTime;
        if(timer > time)
        {
            timer = 0f;
            return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider col)
    {
        if(endFlag == false && col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMove>().Damage();
        }
        else if(col.gameObject.tag == "CableMoveBlock")
        {
            Destroy(gameObject);
        }
    }
}
