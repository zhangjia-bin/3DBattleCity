using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Google.Protobuf;
using PlayerIn;

public class PlayerMove : MonoBehaviour
{
    public AudioSource move;
    public AudioSource bao;
    public AudioSource ge;
    //击杀敌人的数量
    void Start()
    {
       
        move = transform.Find("move").GetComponent<AudioSource>();
        bao = transform.Find("bao").GetComponent<AudioSource>();
    }

    float t;
    float time = 0;
    float time1 = 0;
    bool flag = false;
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 0.1)
        {
            MovePlayer();
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            flag = true;
        }
        else
        {
            move.Stop();
        }
        if (flag)
        {
            if (!move.isPlaying) {
                move.Play();
            }
        }
        transform.Translate(transform.forward * v * Time.deltaTime * 1000, Space.World);
        transform.Rotate(transform.up * h * Time.deltaTime * 100, Space.World);


        Transform w1 = transform.Find("turret_1/pao").transform;
        Debug.DrawRay(w1.position, w1.forward * 10000, Color.red);
        //炮管转向
        if (Input.GetKey(KeyCode.Q))
        {
            transform.GetChild(0).Rotate(new Vector3(0, 1, 0) * Time.deltaTime * -50, Space.World);

        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.GetChild(0).Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 50, Space.World);

        }
        t += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (t > 2.5)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("zidanRole"));
                go.transform.position = w1.position;
                go.transform.forward = w1.forward;
                go.GetComponent<Rigidbody>().AddForce(go.transform.forward * 1500000);
                Destroy(go, 10);
                //时间归零
                t = 0;
                player p = new player();
                p.Id = transform.GetComponent<OtherPlayer>().id;
                NetManage.Ins.OnClientSendToSever(MessageID.C2S_FirePlayer, p.ToByteArray());

                //播放爆炸声效
                bao.Play();
            }
        }

        if (transform.GetComponentInChildren<Slider>().value <= 0)
        {
            SceneManager.LoadScene(2);
        }
    }
    void MovePlayer()
    {
        //不断的发送消息给客户端
        player p = new player();
        p.Px = transform.position.x;
        p.Py = transform.position.y;
        p.Pz = transform.position.z;
        //旋转
        p.Rx = transform.eulerAngles.x;
        p.Ry = transform.eulerAngles.y;
        p.Rz = transform.eulerAngles.z;
        //头角度
        Vector3 w1 = transform.GetChild(0).transform.eulerAngles;
        p.Tx = w1.x;
        p.Ty = w1.y;
        p.Tz = w1.z;
        p.Id = transform.GetComponent<OtherPlayer>().id;
        //发送消息给所有客户端
        NetManage.Ins.OnClientSendToSever(MessageID.C2S_Move, p.ToByteArray());
    }
}
