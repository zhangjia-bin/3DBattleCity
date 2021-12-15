using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Google.Protobuf;
using PlayerIn;

public class HPrecovery : MonoBehaviour
{
    public GameObject fatart;
    void Start()
    {
        
    }
    bool flag = false;
    // Update is called once per frame
    void Update()
    {
        
    }
    float t;
    private void OnCollisionStay(Collision collision)
    {
        t += Time.deltaTime;
        if (t >= 5)
        {
            if (collision.transform.tag == "diren")
            {
                if (collision.transform.GetComponentInChildren<Slider>().value <= 95)
                {
                    player p = new player();
                    collision.transform.GetComponentInChildren<Slider>().value += 5;
                    p.Id = collision.transform.GetComponent<OtherPlayer>().id;
                    p.Boold = 5;
                    //发送消息告诉所有人血量在恢复
                    NetManage.Ins.OnClientSendToSever(MessageID.C2S_Recovery, p.ToByteArray());

                    //血量恢复效果
                    Vector3 pos = Camera.main.WorldToScreenPoint(collision.transform.position);
                    Text xue = Instantiate(Resources.Load<Text>("xue"));
                    xue.text = "+" + 5.ToString();
                    xue.transform.position = pos;
                    Transform father = GameObject.Find("Canvas/Scoreboard").transform;
                    xue.transform.SetParent(father);
                    xue.gameObject.AddComponent<Mao>();
                    Destroy(xue.gameObject, 3);

                    //每过三秒恢复血量
                    t = 2;
                }
                
            }
        }
    }
}
