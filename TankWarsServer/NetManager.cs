using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace AllSever
{

    class NetManager : Singleton<NetManager>
    {
        Socket socket;
        //客户端的集合所在
        public List<Client> listcli = new List<Client>();
        public void Init()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //开启服务器的IP地址以及端口号
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 10086);
            //建立连接
            socket.Bind(ip);
            //设置最大监听数
            socket.Listen(500);
            Console.WriteLine("服务器已开启！");
            //开始接受客户端
            socket.BeginAccept(AcceptCall, null);
        }

        private void AcceptCall(IAsyncResult ar)
        {
            try
            {
                Socket cli = socket.EndAccept(ar);
                IPEndPoint iP = cli.RemoteEndPoint as IPEndPoint;

                Console.WriteLine("IP地址为" + iP.Address + "端口号为：" + iP.Port + "加入服务器");

                Client client = new Client();
                client.socket = cli;
                //赋值唯一的标识符
                client.id = iP.Port;

                listcli.Add(client);

                //开始接受客户端的消息
                cli.BeginReceive(client.data, 0, client.data.Length, SocketFlags.None, ReceivedCall, client);

                //继续接受进入的客户端
                socket.BeginAccept(AcceptCall, null);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }

        private void ReceivedCall(IAsyncResult ar)
        {

            Client cli = ar.AsyncState as Client;
            try
            {
                if (cli.socket.Connected) {
                    IPEndPoint ipend = cli.socket.RemoteEndPoint as IPEndPoint;
                    //实际接受的数据长度
                    int len = cli.socket.EndReceive(ar);
                    if (len > 0)
                    {
                        byte[] rdata = new byte[len];
                        //重新赋值
                        Buffer.BlockCopy(cli.data, 0, rdata, 0, len);
                        //沾包处理
                        cli.my.Write(rdata, 0, rdata.Length);
                        while (cli.my.Length >= 2)
                        {
                            //处理第一个数据的数据流消息
                            cli.my.Position = 0;
                            //通过消息头读出消息的身体
                            int blen = cli.my.ReadUshort();
                            //原先的数据长度
                            int AllLen = blen + 2;
                            if (cli.my.Length - AllLen >= 0)
                            {
                                byte[] Bdata = new byte[blen];
                                cli.my.Read(Bdata, 0, Bdata.Length);

                                //取出消息号
                                int id = BitConverter.ToInt32(Bdata, 0);
                                //取出发送的消息
                                byte[] info = new byte[Bdata.Length - 4];
                                Buffer.BlockCopy(Bdata, 4, info, 0, info.Length);
                                //通过消息中心对事件进行分发
                                MessageManager.Ins.OnSend(id, info, cli);
                                //剩余流的长度
                                int Sylen = (int)cli.my.Length - AllLen;
                                if (Sylen > 0)
                                {
                                    byte[] sydata = new byte[Sylen];
                                    //剩下的字节流读出来
                                    cli.my.Read(sydata, 0, sydata.Length);
                                    //全部归为零（目的是为了重新赋值）
                                    cli.my.SetLength(0);
                                    cli.my.Position = 0;
                                    //重新写入流
                                    cli.my.Write(sydata, 0, sydata.Length);
                                }
                                else
                                {
                                    //全部归为零（目的是为了重新赋值）
                                    cli.my.SetLength(0);
                                    cli.my.Position = 0;
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
                        cli.socket.BeginReceive(cli.data, 0, cli.data.Length, SocketFlags.None, ReceivedCall, cli);
                    }
                    else
                    {
                        if (listcli.Contains(cli))
                        {


                            //下线通知
                            PlayerMove.Ins.Online(cli, PlayerIn.playerstate.Down);
                            listcli.Remove(cli);
                            cli.socket.Shutdown(SocketShutdown.Both);
                            cli.socket.Close();
                            Console.WriteLine("IP为：" + ipend.Address + "客户端口号为" + ipend.Port + "客户端已退出");


                        }

                    }

                }
              
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }

        //消息发送给客户端的作用
        public void OnSeverSendToClient(int id, byte[] info, Client client)
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
                client.socket.BeginSend(alldata, 0, alldata.Length, SocketFlags.None, SendCall, client);
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
                Client cli = ar.AsyncState as Client;
                int len = cli.socket.EndSend(ar);

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
}
