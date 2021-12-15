using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        NetManage.Ins.Start();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        NetManage.Ins.Update();
    }
    private void OnApplicationQuit()
    {
        NetManage.Ins.socket.Shutdown(SocketShutdown.Both);
        NetManage.Ins.socket.Close();
        Debug.Log("客户端已退出！！！");
    }
}
