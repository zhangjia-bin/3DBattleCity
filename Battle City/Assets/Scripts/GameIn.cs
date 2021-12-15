using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Google.Protobuf;
using PlayerIn;
public class GameIn : MonoBehaviour
{
    //客户端的集合
    public  List<GameObject> listAll = new List<GameObject>();
    public GameObject setfather;
    public Text num;
    //击杀敌人的数量

    //出生点
    GameObject[] list;
    void Start()
    {

        list = GameObject.FindGameObjectsWithTag("Ins");

        //请求所有的客户端在线列表
        MessageManager.Ins.AddLister(MessageID.S2C_GetAllClient, S2C_GetAllClient);

        //实例化进入游戏的人物
        MessageManager.Ins.AddLister(MessageID.S2S_CallIns,CallIns);
        //单个客户端进入
        MessageManager.Ins.AddLister(MessageID.S2C_BeginGameCall, S2C_BeginGameCall);


        //人物的移动的回调方法
        MessageManager.Ins.AddLister(MessageID.S2C_MoveCall, S2C_MoveCall);

        //回调发送炮弹的方法
        MessageManager.Ins.AddLister(MessageID.C2S_FirePlayerCall, C2S_FirePlayerCall);

        //击杀人物消息的提示(回调方法)
        MessageManager.Ins.AddLister(MessageID.C2S_KillshowCall, C2S_KillshowCall);

        //发送请求所有的客户端集合
        NetManage.Ins.OnClientSendToSever(MessageID.C2S_GetAllSend, new byte[1]);

        //告诉所有的客户端血量恢复
        MessageManager.Ins.AddLister(MessageID.C2S_RecoveryCall, C2S_RecoveryCall);

    }

    private void C2S_RecoveryCall(object obj)
    {
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];
        player p = player.Parser.ParseFrom(data);
        //找出所要加血的人物
        foreach (var item in listAll)
        {
            if (item.GetComponent<OtherPlayer>().id == p.Id)
            {
                //血量恢复效果
                Vector3 pos = Camera.main.WorldToScreenPoint(item.transform.position);
                Text xue = Instantiate(Resources.Load<Text>("xue"));
                xue.text = "+" + 5.ToString();
                xue.transform.position = pos;
                Transform father = GameObject.Find("Canvas/Scoreboard").transform;
                xue.transform.SetParent(father);
                xue.gameObject.AddComponent<Mao>();
                Destroy(xue.gameObject, 3);
                //血量++
                item.GetComponentInChildren<Slider>().value += 3;
            }
        }
    }

    private void C2S_KillshowCall(object obj)
    {
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];
        KillRen p = KillRen.Parser.ParseFrom(data);
        for (int i = 0; i < listAll.Count; i++)
        {
            if (listAll[i].GetComponent<OtherPlayer>().id == p.Id)
            {
                //删除(玩家)
                Destroy(listAll[i]);
                listAll.Remove(listAll[i]);
            }
        }
        GameObject go = GameObject.Find("Canvas/Scoreboard/Killtip");
        Text show = go.GetComponent<Text>();
        show.text = p.Dr + "被击杀啦！！！" ;
        //击杀消息的提示
        num.text = KillCount.Ins.count.ToString();

    }

    private void C2S_FirePlayerCall(object obj)
    {
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];
        player p = player.Parser.ParseFrom(data);
        //寻找id，告诉他进行移动
        foreach (var item in listAll)
        {
            if (item.GetComponent<OtherPlayer>().id == p.Id)
            {
                //发射炮弹
                item.GetComponent<OtherPlayer>().StartFire();
            }
        }
    }

    private void S2C_MoveCall(object obj)
    {
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];
        player p = player.Parser.ParseFrom(data);
        //寻找id，告诉他进行移动
        foreach (var item in listAll)
        {
            if (item.GetComponent<OtherPlayer>().id == p.Id)
            {
                //位置
                item.GetComponent<OtherPlayer>().SetPos(new Vector3(p.Px,p.Py,p.Pz));
                //身体转
                item.GetComponent<OtherPlayer>().SetRot(new Vector3(p.Rx,p.Ry,p.Rz));
                //头转
                item.GetComponent<OtherPlayer>().SetPaotou(new Vector3(p.Tx,p.Ty,p.Tz));
            }
        }
    }

    private void CallIns(object obj)
    {
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];

        //一开始进入游戏实例化所有人进入
        player p = player.Parser.ParseFrom(data);
        GameObject go = Instantiate(Resources.Load<GameObject>("1"));
        //相机的位置赋值
        Transform w1 = go.transform.Find("turret_1").transform;
        Camera.main.transform.SetParent(w1);
        //出生点位置得设置
        go.transform.position = new Vector3(p.Px,p.Py,p.Pz);
        //所有的客户端的血量赋值
        go.GetComponentInChildren<Slider>().value = p.Boold;
        //添加人物移动的脚本
        go.AddComponent<PlayerMove>();
        //赋值人物的名字
        go.GetComponentInChildren<Text>().text = p.Playername;
        //赋值人物id
        go.GetComponent<OtherPlayer>().id = p.Id;
        //添加入集合
        
        listAll.Add(go);
    }

    private void S2C_BeginGameCall(object obj)
    {
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];

        //告诉所有人进入
        player p = player.Parser.ParseFrom(data);
        switch (p.State)
        {
            case playerstate.Up:
                GameObject go = Instantiate(Resources.Load<GameObject>("2"));
                //出生点位置得设置
                go.transform.position = new Vector3(p.Px, p.Py, p.Pz);
                //所有的客户端的血量赋值
                go.GetComponentInChildren<Slider>().value = p.Boold;
                //赋值人物的名字
                go.GetComponentInChildren<Text>().text = p.Playername;
                //赋值人物id
                go.GetComponent<OtherPlayer>().id = p.Id;
                //添加入集合
                listAll.Add(go);
                break;
            case playerstate.Down:
                foreach (var item in listAll)
                {
                    if (item.GetComponent<OtherPlayer>().id == p.Id) {
                        //退出客户端（移除）
                        Destroy(item);
                        listAll.Remove(item);
                    }
                }
                break;
            default:break;
        }
    }

    //获取到所有的上线的客户端
    private void S2C_GetAllClient(object obj)
    {
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];
        //所有的客户端
        Allplyaers allplyaers = Allplyaers.Parser.ParseFrom(data);
        foreach (var item in allplyaers.Useralls)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("2"));
            //赋值人物的名字
            go.GetComponentInChildren<Text>().text = item.Playername;
            //赋值人物id
            go.GetComponent<OtherPlayer>().id = item.Id;
            //所有的客户端的血量赋值
            go.GetComponentInChildren<Slider>().value = item.Boold;
            //位置
            go.GetComponent<OtherPlayer>().SetPos(new Vector3(item.Px, item.Py, item.Pz));
            //身体转
            go.GetComponent<OtherPlayer>().SetRot(new Vector3(item.Rx, item.Ry, item.Rz));
            //头转
            go.GetComponent<OtherPlayer>().SetPaotou(new Vector3(item.Tx, item.Ty, item.Ty));
            //添加入集合
            listAll.Add(go);
           
        }
    }

}
