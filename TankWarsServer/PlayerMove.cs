using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


using Google.Protobuf;
using PlayerIn;
namespace AllSever
{
    class PlayerMove : Singleton<PlayerMove>
    {
        List<v3cs> V3s = new List<v3cs>();
        public void Init()
        {
            V3s.Add(new v3cs(0f, 0, 0f));
            V3s.Add(new v3cs(0, 0, 6043f));
            V3s.Add(new v3cs(6436.675f, 0, -15298.03f));
            V3s.Add(new v3cs(7104.647f, 0, -25623.21f));
            V3s.Add(new v3cs(-1365.006f, 0, -25246.91f));
            V3s.Add(new v3cs(16433.87f, 0, -21289.76f));
            V3s.Add(new v3cs(5550.903f, 0, 8793.615f));


            MessageManager.Ins.AddLister(MessageID.C2S_BeginGame, C2S_BeginGame);
            //人物移动
            MessageManager.Ins.AddLister(MessageID.C2S_Move, C2S_Move);
            //接受发射炮弹
            MessageManager.Ins.AddLister(MessageID.C2S_FirePlayer, C2S_FirePlayer);

            //击杀消息的提示
            MessageManager.Ins.AddLister(MessageID.C2S_Killshow, C2S_Killshow);

            //要求得到所有的在线列表
            MessageManager.Ins.AddLister(MessageID.C2S_GetAllSend, C2S_GetAllSend);

            //血量同步的作业
            MessageManager.Ins.AddLister(MessageID.C2S_TongbuBoold, C2S_TongbuBoold);

            //血量恢复的代码
            MessageManager.Ins.AddLister(MessageID.C2S_Recovery, C2S_Recovery);
        }

        //血量恢复的代码
        private void C2S_Recovery(object obj)
        {
            object[] arr = obj as object[];
            byte[] data = arr[0] as byte[];
            Client cli = arr[1] as Client;

            player p = player.Parser.ParseFrom(data);
            //发送给所有的客户端谁恢复了血量
            foreach (var item in NetManager.Ins.listcli)
            {
                if (item.id != p.Id)
                {
                    //发送给所有的客户端
                    NetManager.Ins.OnSeverSendToClient(MessageID.C2S_RecoveryCall, p.ToByteArray(), item);
                }
            }
        }
        //血量同步的作业
        private void C2S_TongbuBoold(object obj)
        {
            object[] arr = obj as object[];
            byte[] data = arr[0] as byte[];
            Client cli = arr[1] as Client;

            KillRen ren = KillRen.Parser.ParseFrom(data);
            foreach (var item in NetManager.Ins.listcli)
            {
                if (item.id == ren.Id)
                {
                    item.boold = ren.Bloor;
                }
            }

        }
        //要求得到所有的在线列表
        private void C2S_GetAllSend(object obj)
        {
            object[] arr = obj as object[];
            byte[] data = arr[0] as byte[];
            Client cli = arr[1] as Client;
            //发送所有的在线列表
            GetAllPlayer(cli);
        }

        //击杀消息的提示
        private void C2S_Killshow(object obj)
        {
            object[] arr = obj as object[];
            byte[] data = arr[0] as byte[];
            Client cli = arr[1] as Client;

            KillRen ren = KillRen.Parser.ParseFrom(data);
            for (int i = 0; i < NetManager.Ins.listcli.Count; i++)
            {
                if (NetManager.Ins.listcli[i].id == ren.Id)
                {
                  
                    //下线通知
                    PlayerMove.Ins.Online(cli, PlayerIn.playerstate.Down);
                    //NetManager.Ins.listcli[i].socket.Shutdown(SocketShutdown.Both);
                    //NetManager.Ins.listcli[i].socket.Close();
                    //Console.WriteLine( "客户端口号为" + ren.Id + "客户端已退出");
                    //在客户端移除
                    NetManager.Ins.listcli.Remove(NetManager.Ins.listcli[i]);

                }
            }

            //发送给所有的客户端
            foreach (var item in NetManager.Ins.listcli)
            {
                NetManager.Ins.OnSeverSendToClient(MessageID.C2S_KillshowCall, data, item);

            }
        }
        //接受发射炮弹
        private void C2S_FirePlayer(object obj)
        {
            object[] arr = obj as object[];
            byte[] data = arr[0] as byte[];
            Client cli = arr[1] as Client;

            foreach (var item in NetManager.Ins.listcli)
            {
                if (item.id != cli.id)
                {
                    NetManager.Ins.OnSeverSendToClient(MessageID.C2S_FirePlayerCall, data, item);
                }

            }
        }

        //人物移动
        private void C2S_Move(object obj)
        {
            object[] arr = obj as object[];
            byte[] data = arr[0] as byte[];
            Client cli = arr[1] as Client;

            //发送过来的对象
            player p = player.Parser.ParseFrom(data);
            cli.px = p.Px;
            cli.py = p.Py;
            cli.pz = p.Pz;

            cli.rx = p.Rx;
            cli.ry = p.Ry;
            cli.rz = p.Rz;
            //p.Id = cli.id;

            cli.tx = p.Tx;
            cli.ty = p.Ty;
            cli.tz = p.Tz;
            foreach (var item in NetManager.Ins.listcli)
            {

                if (item.id != p.Id&&item.playername!=null)
                {
                    NetManager.Ins.OnSeverSendToClient(MessageID.S2C_MoveCall, p.ToByteArray(), item);
                }


            }
        }


        //发送消息给所有的客户端我上线；啊
        public void Online(Client cli, playerstate playerstate)
        {
            player p = new player();
            p.Id = cli.id;
            if (cli.playername == null) return;//(没有进入游戏退出客户端)
            p.Playername = cli.playername;
            p.Px = cli.px;
            p.Py = cli.py;
            p.Pz = cli.pz;
            p.Boold = cli.boold;
            p.State = playerstate;
            foreach (var item in NetManager.Ins.listcli)
            {
                //告诉除自己外的所有客户端
                if (item.id != cli.id)
                {
                    NetManager.Ins.OnSeverSendToClient(MessageID.S2C_BeginGameCall, p.ToByteArray(), item);
                }
            }
        }

        //发送所有的列表给进入游戏的玩家
        private void GetAllPlayer(Client cli)
        {
            Allplyaers list = new Allplyaers();
            foreach (var item in NetManager.Ins.listcli)
            {
                //告诉除自己外的所有客户端
                if (item.id != cli.id)
                {
                    player p = new player();
                    p.Id = item.id;
                    if (item.playername == null) continue;
                    p.Playername = item.playername;
                    p.Px = item.px;
                    p.Py = item.py;
                    p.Pz = item.pz;

                    p.Rx = item.rx;
                    p.Ry = item.ry;
                    p.Rz = item.rz;

                    p.Tx = item.tx;
                    p.Ty = item.ty;
                    p.Tz = item.tz;
                    p.Boold = item.boold;
                    ////转换成枚举
                    //p.State = (playerstate)Enum.Parse(typeof(playerstate), cli.state);

                    //所有的集合
                    list.Useralls.Add(p);
                }
            }
            NetManager.Ins.OnSeverSendToClient(MessageID.S2C_GetAllClient, list.ToByteArray(), cli);
        }

        private void C2S_BeginGame(object obj)
        {
            object[] arr = obj as object[];
            byte[] data = arr[0] as byte[];
            Client cli = arr[1] as Client;

            //发送过来的对象
            player p = player.Parser.ParseFrom(data);
            player role = new player();
            //出生点的位置设置
            Random r = new Random();
            int n = r.Next(2);
            v3cs v = V3s[n];
            role.Playername = p.Playername;
            role.Px = v.X;
            role.Py = v.Y;
            role.Pz = v.Z;
            role.Id = cli.id;
            role.Boold = 100;
            role.State = playerstate.Up;
            //赋值给服务器客户端集合
            cli.px = v.X;
            cli.py = v.Y;
            cli.pz = v.Z;
            cli.playername = p.Playername;
            cli.state = playerstate.Up.ToString();
            //初始血量
            cli.boold = 100;



            //发送消息给客户端对人物进行实例化
            NetManager.Ins.OnSeverSendToClient(MessageID.S2S_CallIns, role.ToByteArray(), cli);
            //发送客户端
            Online(cli, playerstate.Up);


        }


    }
}
