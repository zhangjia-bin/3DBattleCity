using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSever
{
    class MessageManager:Singleton<MessageManager>
    {
        Dictionary<int, Action<object>> dic = new Dictionary<int, Action<object>>();

        //添加监听事件
        public void AddLister(int id,Action<object> action)
        {
            if (dic.ContainsKey(id))
            {
                dic[id] += action;
            }
            else
            {
                dic.Add(id,action);
            }
        }
        //派发监听事件
        public void OnSend(int id,params object[] arr)
        {
            if (dic.ContainsKey(id))
            {
                dic[id](arr);
            }
        }
    }
}
