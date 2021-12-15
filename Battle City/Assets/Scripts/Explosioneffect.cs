using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Google.Protobuf;
using PlayerIn;
public class Explosioneffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        //碰到敌人死亡
        if (collision.transform.tag == "diren")
        {
            Destroy(transform.gameObject);
            // Destroy(collision.gameObject);
            collision.transform.GetComponentInChildren<Slider>().value -=25;
            //制作冒血效果！！！

            //冒血的位置
            Vector3 pos = Camera.main.WorldToScreenPoint(collision.transform.position);
            Text xue = Instantiate(Resources.Load<Text>("xue"));
            xue.text = "-"+25.ToString();
            xue.transform.position = pos;
            Transform father = GameObject.Find("Canvas/Scoreboard").transform;
            xue.transform.SetParent(father);
            xue.gameObject.AddComponent<Mao>();
            Destroy(xue.gameObject, 3);

            //发送血量作用是同步血量
            KillRen ren1 = new KillRen();
            ren1.Id = collision.transform.GetComponent<OtherPlayer>().id;
            ren1.Bloor = (int)collision.transform.GetComponentInChildren<Slider>().value;
            //发送消息给服务器
            NetManage.Ins.OnClientSendToSever(MessageID.C2S_TongbuBoold,ren1.ToByteArray());




            //击杀效果提示
            if (collision.transform.GetComponentInChildren<Slider>().value <= 0)
            {
                //击杀敌人消息的发送
                KillRen ren = new KillRen();
                string name1 = GameObject.Find("1(Clone)/Canvas/name").GetComponent<Text>().text;
                string name2 = collision.transform.Find("Canvas/name").GetComponent<Text>().text;
                ren.Wj = name1;
                ren.Dr = name2;
                ren.Id = collision.transform.GetComponent<OtherPlayer>().id;
                //发送给客户端（击杀消息）
                NetManage.Ins.OnClientSendToSever(MessageID.C2S_Killshow, ren.ToByteArray());
                Debug.Log("xxx");
            }

        }
        GameObject go = Instantiate(Resources.Load<GameObject>("BigExplosion"));
        go.transform.position = transform.position;
        Destroy(go.gameObject, 3);
        Destroy(gameObject);

    }
}
