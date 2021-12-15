using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System;

public class NetManage : Singleton<NetManage>
{
  public  Socket socket;
    //缓存区
    byte[] data = new byte[1024];
    //流
    MyMemoryStream my = new MyMemoryStream();
    //队列发消息
    Queue<byte[]> myqueue = new Queue<byte[]>();
    public void Start()
    {

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //开始连接客户端
        socket.BeginConnect("127.0.0.1",10086,ConnectCall,null);
    }

    private void ConnectCall(IAsyncResult ar)
    {
        try
        {
            socket.EndConnect(ar);
            Debug.Log("服务器连接成功");
            //开始接受消息
            socket.BeginReceive(data,0,data.Length,SocketFlags.None,ReceiveCall,null);
        }
        catch (Exception)
        {

            throw;
        }
    }

    private void ReceiveCall(IAsyncResult ar)
    {
        try
        {
            int len = socket.EndReceive(ar);
            if (len > 0)
            {
                byte[] rdata = new byte[len];
                //重新赋值
                Buffer.BlockCopy(data, 0, rdata, 0, len);
                //沾包处理
                my.Write(rdata, 0, rdata.Length);
                while (my.Length >= 2)
                {
                    //处理第一个数据的数据流消息
                    my.Position = 0;
                    //通过消息头读出消息的身体
                    int blen = my.ReadUshort();
                    //原先的数据长度
                    int AllLen = blen + 2;
                    if (my.Length - AllLen >= 0)
                    {
                        byte[] Bdata = new byte[blen];
                        //读出后的数据从流中去除
                        my.Read(Bdata, 0, Bdata.Length);

                        //入队
                        myqueue.Enqueue(Bdata);

                        //剩余流的长度
                        int Sylen = (int)my.Length - AllLen;
                        if (Sylen > 0)
                        {
                            byte[] sydata = new byte[Sylen];
                            //剩下的字节流读出来
                            my.Read(sydata, 0, sydata.Length);
                            //全部归为零（目的是为了重新赋值）
                            my.SetLength(0);
                            my.Position = 0;
                            //重新写入流
                            my.Write(sydata, 0, sydata.Length);
                        }
                        else
                        {
                            //全部归为零（目的是为了重新赋值）
                            my.SetLength(0);
                            my.Position = 0;
                            break;

                        }
                    }
                    else
                    {
                        //没有数据可读
                        break;
                    }
                }
                //继续等待接受客户端的消息
                socket.BeginReceive(data, 0, data.Length, SocketFlags.None, ReceiveCall, null);
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    public void Update()
    {
        while (myqueue.Count > 0)
        {
            byte[] Bdata = myqueue.Dequeue();
            //取出消息号
            int id = BitConverter.ToInt32(Bdata, 0);
            //取出发送的消息
            byte[] info = new byte[Bdata.Length - 4];
            Buffer.BlockCopy(Bdata, 4, info, 0, info.Length);
            //通过消息中心对事件进行分发
            MessageManager.Ins.OnSend(id, info);
        }

    }

    //消息发送给服务器的作用
    public void OnClientSendToSever(int id, byte[] info)
    {
        try
        {
            //要发送的客户端
            byte[] alldata = new byte[4 + info.Length];

            byte[] idbyte = BitConverter.GetBytes(id);
            //重新赋值
            Buffer.BlockCopy(idbyte, 0, alldata, 0, idbyte.Length);
            Buffer.BlockCopy(info, 0, alldata, idbyte.Length, info.Length);
            //制作的消息号
            alldata = MakeData(alldata);
            socket.BeginSend(alldata, 0, alldata.Length, SocketFlags.None, SendCall, null);
        }
        catch (Exception e)
        {

            Console.WriteLine(e);
        }
    }

    private void SendCall(IAsyncResult ar)
    {
        try
        {
            int len = socket.EndSend(ar);

        }
        catch (Exception)
        {
            throw;
        }
    }

    //制作消息体的作用
    public byte[] MakeData(byte[] dataAllcli)
    {
        using (MyMemoryStream my1 = new MyMemoryStream())
        {
            //写入消息头
            my1.WriteUShort((ushort)dataAllcli.Length);
            //把身体写入
            my1.Write(dataAllcli, 0, dataAllcli.Length);
            //制作好的消息返回
            return my1.ToArray();
        }
    }

  
}
