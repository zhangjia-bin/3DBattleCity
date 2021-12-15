using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Google.Protobuf;
using PlayerIn;
public class OtherPlayer : MonoBehaviour
{
    public int id;
    public AudioSource move;
    public AudioSource bao;
    private void Start()
    {
        move = transform.Find("move").GetComponent<AudioSource>();
        bao = transform.Find("bao").GetComponent<AudioSource>();
    }
    //设置人物的位置
    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
        //播放声音
       
    }
    //设置坦克的转向
    public void SetRot(Vector3 eulerAngles)
    {
        transform.eulerAngles = eulerAngles;
    }
    //设置炮口的转向
    public void SetPaotou(Vector3 zhuan)
    {

        transform.GetChild(0).transform.eulerAngles = zhuan;
        Transform w1 = transform.GetChild(0);
        Debug.DrawRay(w1.position, w1.forward * 10000, Color.red);
    }
    //发射炮弹
    public void StartFire()
    {
        Transform w1 = transform.Find("turret_1/pao");
        GameObject go = Instantiate(Resources.Load<GameObject>("zidan"));
        go.transform.position = w1.position;
        go.transform.forward = w1.forward;
        go.GetComponent<Rigidbody>().AddForce(go.transform.forward * 1500000);
        Destroy(go, 10);
        bao.Play();
    }
    void Update()
    {

    }
}
