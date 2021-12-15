using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace AllSever
{
    class Client
    {
        //唯一的标识符ID
        public int id;
        //人物取的名字
        public string playername;
        //人物的位置
        public float px, py, pz, rx, ry, rz,tx,ty,tz;
        //在线的转态
        public string state;

        //血量
        public float boold;

        public Socket socket;
        public byte[] data = new byte[1024];
        public MyMemoryStream my = new MyMemoryStream();
    }
}
