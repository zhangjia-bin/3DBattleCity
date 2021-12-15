using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSever
{
    class MessageID
    {

        //请求所有的客户端
        public const int C2S_GetAllSend = 996;
        //发送炮弹
        public const int C2S_FirePlayer = 997;
        public const int C2S_FirePlayerCall = 998;

        //刚刚登录游戏对游戏对象进行实例化
        public const int S2S_CallIns = 999;

        public const int C2S_BeginGame = 1001;
        public const int S2C_BeginGameCall = 1002;

        //进入游戏获取所有的在线客户端
        public const int S2C_GetAllClient = 1000;


        //人物的移动
        public const int C2S_Move = 1003;
        public const int S2C_MoveCall = 1004;

        //击杀人物消息的一系列提示
        public const int C2S_Killshow = 1005;
        public const int C2S_KillshowCall = 1006;


        //玩家死亡后的处理
        public const int C2S_Death = 1007;
        //从实例化对象中移除
        public const int C2S_RemoveObj = 1008;

        //同步血量
        public const int C2S_TongbuBoold = 1009;

        //血量恢复
        public const int C2S_Recovery = 1010;
        public const int C2S_RecoveryCall = 1011;
    }
}
